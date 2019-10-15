using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using model = PmsSmi.Data.Model;
namespace PmsSmi.Formatters
{
    public partial class XlsxFormatter : OutputFormatter
    {
        HashSet<string> _contentTypes = new HashSet<string> { "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
        public XlsxFormatter()
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
            var response = context.HttpContext.Response;
            using (var formatter = new XlsxFormatterInternal())
            {
                formatter.LoadTemplate();
                formatter.WriteData(context.Object, response.Body);
            }


            return Task.Delay(100);
        }
        public class XlsxFormatterInternal : IDisposable
        {
            static readonly RuntimeTypeHandle runtimeTypeHandle = typeof(XlsxFormatter).TypeHandle;
            MemoryStream workingCopy = new MemoryStream();
            public void LoadTemplate()
            {
                const string resName = "template.xlsx";
                var type = Type.GetTypeFromHandle(runtimeTypeHandle);
                using (var stream = type.Assembly.GetManifestResourceStream(type, resName))
                {
                    stream.CopyTo(workingCopy);
                    stream.Flush();
                }
            }
            static readonly XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            void WritePackage(XNode sheetDataNode, Stream stream)
            {
                XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
                using (var xlPackage = Package.Open(workingCopy, FileMode.Open, FileAccess.ReadWrite))
                {
                    var part = xlPackage.GetParts().FirstOrDefault(x => x.Uri.ToString() == "/xl/worksheets/sheet1.xml");
                    using var partStream = part.GetStream(FileMode.Open, FileAccess.ReadWrite);
                    var xdoc = XDocument.Load(partStream, LoadOptions.None);
                    var el = (from x in xdoc.Descendants() where x.Name.Namespace == ns && x.Name.LocalName == "sheetData" select x).FirstOrDefault();

                    el.ReplaceWith(sheetDataNode);
                    partStream.Seek(0, SeekOrigin.Begin);
                    xdoc.Save(partStream);

                }

                {
                    workingCopy.Seek(0, SeekOrigin.Begin);
                    workingCopy.WriteTo(stream);
                    workingCopy.Flush();
                }

            }
            public void WriteData(IEnumerable<model.WorkflowItem> data, Stream stream)
            {
                XElement newNode = new XElement(ns + "sheetData");
                int i = 1;
                foreach (var d in data)
                {
                    XElement row = new XElement(ns + "row", new XAttribute($"r", i));
                    row.Add(new XElement(ns + "c", new XAttribute($"r", $"A{i}"), new XElement(ns + "v", $"{d.ItemType}")));
                    row.Add(new XElement(ns + "c", new XAttribute($"r", $"B{i}"), new XElement(ns + "v", $"{d.Name}")));
                    row.Add(new XElement(ns + "c", new XAttribute($"r", $"C{i}"), new XElement(ns + "v", $"{d.StartDate}")));
                    newNode.Add(row);
                    i++;
                }
                WritePackage(newNode, stream);
            }
            public void WriteData(object data, Stream stream)
            {
                var d = data as IEnumerable<model.WorkflowItem>;
                if (d != null)
                {
                    WriteData(d, stream);
                    return;
                }
                XElement newNode = new XElement(ns + "sheetData");

                Enumerable.Range(1, 10).ToList().ForEach(i =>
                {
                    XElement row = new XElement(ns + "row", new XAttribute($"r", i));
                    row.Add(new XElement(ns + "c", new XAttribute($"r", $"A{i}"), new XElement(ns + "v", $"{i}")));
                    row.Add(new XElement(ns + "c", new XAttribute($"r", $"B{i}"), new XElement(ns + "v", $"{i}")));
                    row.Add(new XElement(ns + "c", new XAttribute($"r", $"C{i}"), new XElement(ns + "v", $"{i}")));
                    newNode.Add(row);
                });
                WritePackage(newNode, stream);

            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        workingCopy?.Dispose();
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~XlsxFormatter()
            // {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }


    }


}
