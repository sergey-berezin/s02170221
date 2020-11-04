using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class PictureInfoDB
    {
        public int Id { get; set; }

        public string ShortFileName { get; set; }

        public PictureTypeDB Type { get; set; }

        public PictureInfoDetails PictureInfoDetails { get; set; }
    }

    public class PictureInfoDetails
    {
        public int Id {get; set; }

        public byte[] BinaryFile { get; set; }
    }

    public class PictureTypeDB
    {
        public int Id { get; set; }

        public string TypeName { get; set; }
        
        public ICollection<PictureInfoDB> Pictures { get; set; }
    }


    public class PictureLibraryContext : DbContext
    {
        public DbSet<PictureInfoDB> Pictures { get; set; }

        public DbSet<PictureTypeDB> Types { get; set; }

        public DbSet<PictureInfoDetails> Details { get; set; }


        [NotMapped]
        public List<string> UnknownPictures = new List<string>();

        protected override void OnConfiguring(DbContextOptionsBuilder o) => o.UseSqlite("Data Source=../../../PictureLibrary.db");

        public PictureInfo FindPicture(string file)
        {
            var shortfile = file.Split('/').LastOrDefault();
            if (Pictures.Where(p => p.ShortFileName == shortfile).Count() == 0)
            {
                UnknownPictures.Add(file);
                return null;
            }

            var binaryfile = GetBinaryFile(file);
            PictureInfoDB buffer = null;
            foreach (var p in Pictures.Where(p => p.ShortFileName == shortfile))
            {
                Entry(p).Reference("PictureInfoDetails").Load();
                if (CompareBinaryFiles(p.PictureInfoDetails.BinaryFile, binaryfile))
                {
                    buffer = p;
                    break;
                }
            }
            if (buffer == null)
            {
                UnknownPictures.Add(file); 
                return null;
            }
            Entry(buffer).Reference("Type").Load();
            return new PictureInfo(file, buffer.Type.TypeName);
        }

        public void AddPictureInfo(PictureInfo pictureInfo)
        {

            var p = new PictureInfoDB();
            p.ShortFileName = pictureInfo.Path.Split('/').Last();
            p.PictureInfoDetails = new PictureInfoDetails();
            p.PictureInfoDetails.BinaryFile = GetBinaryFile(pictureInfo.Path);

            var query = Types.Where(p => pictureInfo.TypeName == p.TypeName); //узнаем, есть ли такой тип в таблице Types
            if (query.Count() > 0)
                p.Type = query.First();
            else
            {
                p.Type = new PictureTypeDB();
                p.Type.TypeName = pictureInfo.TypeName;
                Types.Add(p.Type);
            }

            Details.Add(p.PictureInfoDetails);
            Pictures.Add(p);
            SaveChanges();
        }

        public void ClearDB()
        {
            foreach (var p in Pictures)
                Pictures.Remove(p);
            foreach (var t in Types)
                Types.Remove(t);
            foreach (var d in Details)
                Details.Remove(d);

            SaveChanges();
        }

        private byte[] GetBinaryFile(string filename)
        {
            Stream stream = System.IO.File.OpenRead(filename);
            var binaryImage = new byte[stream.Length];
            stream.Read(binaryImage, 0, (int)stream.Length);
            return binaryImage;
        }

        private bool CompareBinaryFiles(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }

        public int GetStatisticType(string typeName)
        {
            var t = Types.Where(t => t.TypeName == typeName).FirstOrDefault();
            if (t == null)
                return 0;
            Entry(t).Collection("Pictures").Load(); //отложенная загрузка картинок для найденного типа
            return t.Pictures.Count();
        }

    }
}
