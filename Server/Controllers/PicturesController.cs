﻿using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NeuralNetwork;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PicturesController : ControllerBase
    {
        private PictureLibraryContext pictureLibraryContext;
        private MNIST mNIST;
        public PicturesController(PictureLibraryContext pictureLibraryContext)
        {
            this.pictureLibraryContext = pictureLibraryContext;
            mNIST = new MNIST();
        }

        [HttpGet]
        public Transfer[] Get()
        {           
            return pictureLibraryContext.GetAllContent().ToArray();
        }

        [HttpGet("{pageNumber}")]
        public Transfer[] Get(int pageNumber)
        {
            List<Transfer> answer = new List<Transfer>();
            var result = pictureLibraryContext.GetAllContent().ToList();
            for (int i = pageNumber * 10; i < Math.Min((pageNumber + 1) * 10, result.Count); i++)
                answer.Add(result[i]);
            return answer.ToArray();
        }

        [HttpPut]
        public string Put(Transfer transfer) //[FromBody]
        {
            string findRequest = null;
            lock (pictureLibraryContext)
                findRequest = pictureLibraryContext.FindPicture(transfer);
            if (!string.IsNullOrEmpty(findRequest))
                return findRequest;
            Bitmap bmp;
            using (var ms = new MemoryStream(Convert.FromBase64String(transfer.DataToBase64)))
            {
                bmp = new Bitmap(ms);
            }
            bmp = Utils.ResizeBitmap(bmp, 28, 28);
            var result = mNIST.ProcessPicture(bmp);
            transfer.Name = result;
            lock (pictureLibraryContext)
                pictureLibraryContext.AddPictureInfo(transfer);
            return result;
        }

        [HttpDelete]
        public void Delete()
        {
            lock (pictureLibraryContext)
                pictureLibraryContext.ClearDB();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController : ControllerBase
    {
        private PictureLibraryContext pictureLibraryContext;
        private MNIST mNIST;

        public StatisticController(PictureLibraryContext pictureLibraryContext)
        {
            this.pictureLibraryContext = pictureLibraryContext;
            mNIST = new MNIST();
        }
        [HttpGet]
        public Transfer[] Get()
        {
            return pictureLibraryContext.GetAllStatistic().ToArray();
        }
    }

}
