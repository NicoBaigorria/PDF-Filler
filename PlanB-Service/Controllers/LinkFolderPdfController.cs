using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlanB_Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LinkFolderPdfController : ControllerBase
    {
        [HttpGet("{id_folder}")]
        public string Get(string id_folder)
        {
            FormFiller formFiller = new FormFiller();

            return formFiller.GetLinksPdfs(id_folder);
        }
    }
}
