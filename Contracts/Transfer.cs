using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    /// <summary>
    /// Универсальный пакет для пересылки любых данных 
    /// </summary>
    public class Transfer 
    {
        public string DataToBase64 { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }
    }
}
