using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common.Swagger12
{
    /// <summary>
    /// A lookup table of primitive types and their underlying Type/Format in Swagger. 
    /// </summary>
    public class PrimitiveTypeFormat
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public PrimitiveTypeFormat()
        {
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="type"></param>
        /// <param name="format"></param>
        public PrimitiveTypeFormat(string primitive, string type, string format)
        {
            Primitive = primitive;
            Type = type;
            Format = format;
        }

        /// <summary>
        /// The primitive. ie: integer; dateTime
        /// </summary>
        public string Primitive { get; set; }

        /// <summary>
        /// The type. ie: integer
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The format. ie: string; or null
        /// </summary>
        public string Format { get; set; }
    }
}
