using BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using Python.Runtime;
namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubeController : ControllerBase
    {
        CubeSolver cs;
        Cube myCube;
        [HttpPost("getStatus")]
        public ActionResult<List<ActionInCube>> GetStatus([FromBody] string [][][] cube)
        {
            cs = new CubeSolver();
            myCube = new Cube();
            myCube.strToEnum(cube);
            List<ActionInCube> list =cs.run(myCube);
            return Ok(list);
        }
        [HttpPost("buildHeuristic")]
        public void buildHeuristic()
        {
            cs = new CubeSolver();
            cs.build_heuristic_db();      
        }
  
       
    }
}