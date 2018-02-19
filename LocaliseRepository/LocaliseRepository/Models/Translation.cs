using System;
using System.Collections.Generic;
using System.Text;

namespace LocaliseApi.Models
{
    public class TranslationData
    {
		public string Id { get; set; }
		public string Type { get; set; }
		
		public string Translation {get; set; }

		public Locale Locale { get; set; }
    }
}
