using FoodDefence.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.Repository
{
    public class UsuarioRepository
    {

        public LoginResponse Login(string nombre, string clave, string mail)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                LoginResponse user = new LoginResponse();

                USUARIO uSUARIO = new USUARIO();
                uSUARIO = db.USUARIO.Where(
                    w => w.nombre.Trim().ToUpper() == nombre.Trim().ToUpper() &&
                    w.clave.Trim() == clave.Trim() &&
                    w.mail.Trim().ToUpper() == mail.Trim().ToUpper()
                    ).FirstOrDefault();

                if (uSUARIO == null)
                {
                    user.mensaje = "No se encontro el usuario";
                    user.success = false;
                    return user;
                }
                               
                switch (uSUARIO.idUsuarioTipo)
                {
                    case 1:
                        user.mensaje = "";
                        user.success = true;
                        break;
                    case 2:
                        user.mensaje = "Su usuario no tiene permisos";
                        user.success = false;
                        break;
                    case 3:
                        if (uSUARIO.idEmpleado == null || uSUARIO.idEmpleado == 0)
                        {
                            user.mensaje = "Su usuario no esta asociado a un técnico";
                            user.success = false;
                        }
                        else
                        {
                            user.mensaje = "";
                            user.success = true;
                        }
                        break;
                    default:
                        user.mensaje = "Su usuario no tiene definido el tipo";
                        user.success = false;
                        break;
                }

                user.id = uSUARIO.id;
                user.idEmpleado = uSUARIO.idEmpleado;
                user.idUsuarioTipo = uSUARIO.idUsuarioTipo;
                user.mail = uSUARIO.mail;
                user.nombre = uSUARIO.nombre;

                return user;
            }
        }

        public EmployeeResponse GetEmpleado(int id)
        {
            EmployeeResponse emp = new EmployeeResponse();
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                EMPLEADO uEmp = new EMPLEADO();
                uEmp = db.EMPLEADO.Where(w => w.id == id).FirstOrDefault();
                if (uEmp == null)
                    throw new Exception("no se encontro tecnico");
                emp.id = id;
                emp.apellido = uEmp.apellido;                
                emp.nombre = uEmp.nombre;
            }

            return emp;
        }

        public List<EmployeeResponse> GetEmpleados()
        {
            
            List<EmployeeResponse> emps = new List<EmployeeResponse>();
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                List<EMPLEADO> uEmp = new List<EMPLEADO>();
                uEmp = db.EMPLEADO.Where(w => w.baja == false || w.baja == null).ToList();
                if (uEmp == null)
                    throw new Exception("no se encontro usuario");

                emps = (from p in uEmp
                        select new EmployeeResponse
                        {
                            apellido = p.apellido,
                            id = p.id,
                            nombre = p.nombre,
                            nickName = p.nombre.Substring(0, 1).ToUpper() + p.apellido.Substring(0, 1).ToUpper()
            }).ToList();

            }

            return emps;
        }

        public UsuarioResponse GetUsuario(int id)
        {
            UsuarioResponse user = new UsuarioResponse();
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                USUARIO uSUARIO = new USUARIO();
                uSUARIO = db.USUARIO.Where(w => w.id == id).FirstOrDefault();
                if (uSUARIO == null)
                    throw new Exception("no se encontro usuario");
                user.id = id;
                user.idEmpleado = uSUARIO.idEmpleado;
                user.idUsuarioTipo = uSUARIO.idUsuarioTipo;
                user.mail = uSUARIO.mail;
                user.nombre = uSUARIO.nombre;                
            }

            return user;
        }
        
        public List<EmpListTaskResponse> GetEmpListTask(int id, DateTime? fechaDeTrabajo)
        {

            List<EmpListTaskResponse> elt = new List<EmpListTaskResponse>();
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                List<st_getEmpListTask_Result> uElt = new List<st_getEmpListTask_Result>();
                uElt = db.st_getEmpListTask(id, fechaDeTrabajo).ToList();
                if (uElt == null)
                    throw new Exception("no se encontro datos.");

                elt = (from p in uElt
                       select new EmpListTaskResponse
                       {
                           idCliente = p.CLIENTE_ID,
                           razonSocial = p.razonSocial,
                           idSector = p.SECTOR_ID,
                           sector = p.SECTOR,
                           tipoTrampa = p.TIPO_TRAMPA,
                           cnt = (int)p.CNT
                       }).ToList();
            }
            return elt;
        }
    }
}