using FoodDefence.Models.Repository;
using FoodDefence.Models.Request;
using FoodDefence.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace FoodDefence.Controllers
{
    [RoutePrefix("api/usuario")]
    public class rUsuarioController : ApiController
    {


        [HttpGet]
        [Route("get/{id}")]
        public IHttpActionResult Get(int id)
        {
            UsuarioResponse user = new UsuarioResponse();
            
            try
            {
                if (id == 0)
                {
                    return BadRequest("Los datos ingresados no son correctos");
                }
                UsuarioRepository oRep = new UsuarioRepository();
                user = oRep.GetUsuario(id);
                return Ok(user);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
            
        }

        [HttpGet]
        [Route("getemployee/{id}")]
        public IHttpActionResult    getEmployee(int id)
        {
            EmployeeResponse emp = new EmployeeResponse();

            try
            {
                if (id == 0)
                {
                    return BadRequest("Los datos ingresados no son correctos");
                }
                UsuarioRepository oRep = new UsuarioRepository();
                emp = oRep.GetEmpleado(id);

                emp.nickName = emp.nombre.Substring(0, 1).ToUpper() + emp.apellido.Substring(0, 1).ToUpper();
                return Ok(emp);
            }
            catch (Exception)
            {
                return InternalServerError();
            }

        }

        [HttpGet]
        [Route("getemployees")]
        public IHttpActionResult getEmployees()
        {
            List<EmployeeResponse> emps = new List<EmployeeResponse>();

            try
            {                
                UsuarioRepository oRep = new UsuarioRepository();
                emps = oRep.GetEmpleados();
                return Ok(emps);
            }
            catch (Exception)
            {
                return InternalServerError();
            }

        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login([FromBody]UsuarioRequest model)
        {
            LoginResponse user = new LoginResponse();

            try
            {
                if (model.nombre == "" || model.clave == "" || model.mail == "")
                {
                    return BadRequest("Los datos ingresados no son correctos");
                }
                UsuarioRepository oRep = new UsuarioRepository();
                string version = WebConfigurationManager.AppSettings["app_version"].ToString();
                if (version != model.version)
                {
                    user.mensaje = "La version no es correcta, debe actualizar la aplicación";
                    user.success = false;
                    return Ok(user);
                }

                user = oRep.Login(model.nombre, model.clave, model.mail);
                return Ok(user);
            }
            catch (Exception)
            {
                return InternalServerError();
            }

        }


        [HttpGet]
        [Route("getemplisttask/{id}/{fechaDeTrabajo}")]
        public IHttpActionResult getEmpListTask(int id, Nullable<DateTime> fechaDeTrabajo)
        {
            List<EmpListTaskResponse> elt = new List<EmpListTaskResponse>();

            // try
            // {
            UsuarioRepository oRep = new UsuarioRepository();
            elt = oRep.GetEmpListTask(id, fechaDeTrabajo);
            return Ok(elt);
            // }
            // catch (Exception)
            // {
            //     return InternalServerError();
            // }

        }

        // GET: api/rUsuario
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/rUsuario/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/rUsuario
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/rUsuario/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/rUsuario/5
        //public void Delete(int id)
        //{
        //}
    }
}
