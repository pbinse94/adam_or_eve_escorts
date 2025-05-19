using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

[Route("sitemap.xml")]
public class SitemapController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var xml = new XDocument(
            new XElement("urlset",
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.sitemaps.org/schemas/sitemap/0.9"),
                new XElement("url",
                    new XElement("loc", "https://yourdomain.com/"),
                    new XElement("lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                    new XElement("changefreq", "weekly"),
                    new XElement("priority", "1.0")
                )
            )
        );

        return Content(xml.ToString(), "application/xml");
    }
}
