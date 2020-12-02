using Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class PictureInfoDB
    {
        public int Id { get; set; }

        public int HashCode { get; set; }

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


        //[NotMapped]
        //public List<string> UnknownPictures = new List<string>();

        protected override void OnConfiguring(DbContextOptionsBuilder o) => o.UseSqlite("Data Source=../../../PictureLibrary.db");

        public string FindPicture(Transfer transfer)
        {
            if (Pictures.Where(p => p.HashCode == transfer.DataToBase64.GetHashCode()).Count() == 0)
                return null;

            foreach (var p in Pictures.Where(p => p.HashCode == transfer.DataToBase64.GetHashCode()))
            {
                Entry(p).Reference("PictureInfoDetails").Load();
                if (Convert.ToBase64String(p.PictureInfoDetails.BinaryFile) == transfer.DataToBase64)
                {
                    Entry(p).Reference("Type").Load();
                    return p.Type.TypeName;
                }
            }
            return null;
        }

        public void AddPictureInfo(Transfer transfer)
        {
            var p = new PictureInfoDB();
            p.PictureInfoDetails = new PictureInfoDetails();

            var query = Types.Where(p => transfer.Name == p.TypeName); //узнаем, есть ли такой тип в таблице Types
            if (query.Count() > 0)
                p.Type = query.First();
            else
            {
                p.Type = new PictureTypeDB();
                p.Type.TypeName = transfer.Name;
                Types.Add(p.Type);
            }
            p.PictureInfoDetails.BinaryFile = Convert.FromBase64String(transfer.DataToBase64);
            p.HashCode = transfer.DataToBase64.GetHashCode();
            Details.Add(p.PictureInfoDetails);
            Pictures.Add(p);
            SaveChanges();
        }

        public void ClearDB()
        {
            foreach (var p in Pictures)
                Pictures.Remove(p);
            foreach (var d in Details)
                Details.Remove(d);

            SaveChanges();
        }

        public IEnumerable<Transfer> GetAllStatistic()
        {
            foreach (var t in Types.Include((r) => r.Pictures))
            {
                Transfer transfer = new Transfer();
                transfer.DataToBase64 = t.Pictures.Count().ToString();
                transfer.Name = t.TypeName;
                yield return transfer;
            }
        }

        public IEnumerable<Transfer> GetAllContent()
        {
            foreach (var p in Pictures)
            {
                Entry(p).Reference("PictureInfoDetails").Load();
                Entry(p).Reference("Type").Load();
                var transfer = new Transfer();
                transfer.DataToBase64 = Convert.ToBase64String(p.PictureInfoDetails.BinaryFile);
                transfer.Name = p.Type.TypeName;
                yield return transfer;
            }
        }

    }
}
