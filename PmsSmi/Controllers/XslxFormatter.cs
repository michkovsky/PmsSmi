using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using model = PmsSmi.Data.Model;
namespace PmsSmi.Controllers
{
    public class XslxFormatter : OutputFormatter
    {
        HashSet<string> _contentTypes = new HashSet<string> { "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
        public XslxFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/vnd.ms-excel"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }
        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            var type = context.HttpContext.Request.Headers["accept"];
            var ret = SupportedMediaTypes.Contains(type);
            ret &= base.CanWriteResult(context);
            return ret;
        }
        protected override bool CanWriteType(Type type)
        {
            if (typeof(model.WorkflowItem).IsAssignableFrom(type)
                || typeof(IEnumerable<model.WorkflowItem>).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }
            return false;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            
            throw new NotImplementedException("smi exception");
        }
    }
}
