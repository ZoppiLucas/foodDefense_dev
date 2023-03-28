using FoodDefence.Models.objectModel;
using FoodDefence.Models.Request;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FoodDefence.Models.Repository
{
    public class CargaTrabajoRepository
    {
        private List<Trampas_OrdenTrabajo> getTrampasOrdenTrabajo(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();

            List<Trampas_OrdenTrabajo> lstTrampas = (from p in db.st_getDetalleTrampasOrdenTrabajo(id)
                                                     select new Trampas_OrdenTrabajo
                                                     {
                                                         numero = p.numero,
                                                         descripcion = p.descripcion,
                                                         idTrampaClienteLocacionSector = (int)p.idTrampaClienteLocacionSector,
                                                         nueva = "N",
                                                         descTipoTrampa = p.descTipoTrampa,
                                                         idTipoTrampa = (int)p.idTipoTrampa,
                                                         idOrdenTrabajoDetalle = (int)p.idOrdenTrabajoDetalle,
                                                         cargado = p.cargado == null ? false : (bool)p.cargado,
                                                         orden = (int)p.orden
                                                     }).OrderBy(o => o.orden).ToList();
            return lstTrampas;
        }
        public Trampas_OrdenTrabajo getTrampaByNumero(int id, string numeroTrampa, int tipo)
        {
            List<Trampas_OrdenTrabajo> lstTrampa = new List<Trampas_OrdenTrabajo>();
            lstTrampa = getTrampasOrdenTrabajo(id);

            return lstTrampa.Find(a => a.numero.Trim().ToLower() == numeroTrampa.Trim().ToLower() && a.idTipoTrampa == tipo);

        }
        public List<TipoTrampas_OrdenTrabajo> getTrampaGroupTipos(int id)
        {
            List<Trampas_OrdenTrabajo> lstTrampa = new List<Trampas_OrdenTrabajo>();
            lstTrampa = getTrampasOrdenTrabajo(id);

            IEnumerable<TipoTrampas_OrdenTrabajo> trampasGroup = lstTrampa.GroupBy(g => new { g.idTipoTrampa, g.descTipoTrampa }).Select(
                    tipo => new TipoTrampas_OrdenTrabajo()
                    {
                        idTipoTrampa = tipo.Key.idTipoTrampa,
                        descTipoTrampa = tipo.Key.descTipoTrampa,
                        trampas = tipo.GroupBy(t => new { t.idTrampaClienteLocacionSector, t.descripcion, t.numero, t.idOrdenTrabajoDetalle, t.cargado, t.orden }).Select(
                            trampa => new Trampas_OrdenTrabajo()
                            {
                                descripcion = trampa.Key.descripcion,
                                idOrdenTrabajoDetalle = trampa.Key.idOrdenTrabajoDetalle,
                                idTrampaClienteLocacionSector = trampa.Key.idTrampaClienteLocacionSector,
                                numero = trampa.Key.numero,
                                cargado = trampa.Key.cargado,
                                orden = trampa.Key.orden
                            }).OrderBy(o => o.orden).ToList()
                    }
                );

            return trampasGroup.ToList();            
        }
        public Trampas_OrdenTrabajoDetalle getTrampaDetalle(int id)
        {
            
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();

            List<int> list = new List<int>();
            
            Trampas_OrdenTrabajoDetalle result = (from otd in db.ORDEN_TRABAJO_DETALLE
                                                  join tcls in db.TRAMPA_CLIENTE_LOCACION_SECTOR on otd.idTrampaClienteLocacionSector equals tcls.id
                                                  join t in db.TRAMPA on tcls.idTrampa equals t.id
                                                  join tt in db.TRAMPA_TIPO on t.idTrampaTipo equals tt.id
                                                  join cls in db.CLIENTE_LOCACION_SECTOR on tcls.idClienteLocacionSector equals cls.id
                                                  where otd.id == id
                                                  select new Trampas_OrdenTrabajoDetalle
                                                  {
                                                      cargado = (bool)otd.cargado,
                                                      descripcion = "Sector: " + cls.descripcion + ", Tipo Trampa: " + tt.descripcion + " " + t.numero,
                                                      descTipoTrampa = tt.descripcion,
                                                      idAccion = otd.idAccion,                                                      
                                                      idOrdenTrabajo = otd.idOrdenTrabajo,
                                                      idOrdenTrabajoDetalle = otd.id,
                                                      idTipoTrampa = tt.id,
                                                      idTrampaClienteLocacionSector = otd.idTrampaClienteLocacionSector,
                                                      mariposas = otd.mariposas == null ? 0 : (int)otd.mariposas,
                                                      minusculos = otd.minusculos == null ? 0 : (int)otd.minusculos,
                                                      moscas = otd.moscas == null ? 0 : (int)otd.moscas,
                                                      mosquitas = otd.mosquitas == null ? 0 : (int)otd.mosquitas,
                                                      polillas = otd.polillas == null ? 0 : (int)otd.polillas,
                                                      roedor = otd.roedor == null ? 0 : (int)otd.roedor,
                                                      insecto = otd.insecto == null ? 0 : (int)otd.insecto,
                                                      cucaAmericana = otd.cucaAmericana == null ? 0 : (int)otd.cucaAmericana,
                                                      cucaGermanica = otd.cucaGermanica == null ? 0 : (int)otd.cucaGermanica,
                                                      cantidad = otd.cantidad == null ? 0 : (int)otd.cantidad,
                                                      numero = t.numero,
                                                      observaciones = otd.observaciones,
                                                      is_cargado = otd.cargado == true ? "S" : "N",
                                                      multiSelTipoTrampa = tt.multiSel,
                                                      listEstado = list

                                                  }).First();

            return result;
        }

        public List<TipoTrampas_OrdenTrabajoDetalle> getTrampasGroupTipos(int idOrdenTrabajo)
        {
            List<Trampas_OrdenTrabajoDetalle> lstTrampa = new List<Trampas_OrdenTrabajoDetalle>();
            lstTrampa = getTrampasDetalleByidOrdenTrabajo(idOrdenTrabajo);

            IEnumerable<TipoTrampas_OrdenTrabajoDetalle> trampasGroup = lstTrampa.GroupBy(g => new { g.idTipoTrampa, g.descTipoTrampa }).Select(
                    tipo => new TipoTrampas_OrdenTrabajoDetalle()
                    {
                        idTipoTrampa = tipo.Key.idTipoTrampa,
                        descTipoTrampa = tipo.Key.descTipoTrampa,
                        trampas = tipo.GroupBy(t => new
                        {
                            t.idTrampaClienteLocacionSector,
                            t.descripcion,
                            t.numero,
                            t.idOrdenTrabajoDetalle,
                            t.cargado,
                            t.cantidad,
                            t.idAccion,
                            t.idEstado,
                            t.mariposas,
                            t.minusculos,
                            t.moscas,
                            t.observaciones,
                            t.mosquitas,
                            t.polillas,
                            t.roedor,
                            t.insecto,
                            t.cucaAmericana,
                            t.cucaGermanica,
                            t.descripcionAccion,
                            t.descripcionEstado,
                            t.descripcionAccionAbr,
                            t.descripcionEstadoAbr,
                            t.is_cargado
                        }).Select(
                            trampa => new Trampas_OrdenTrabajoDetalle()
                            {
                                descripcion = trampa.Key.descripcion,
                                idOrdenTrabajoDetalle = trampa.Key.idOrdenTrabajoDetalle,
                                idTrampaClienteLocacionSector = trampa.Key.idTrampaClienteLocacionSector,
                                numero = trampa.Key.numero,
                                cargado = trampa.Key.cargado,
                                cantidad = trampa.Key.cantidad,
                                mariposas = trampa.Key.mariposas,
                                minusculos = trampa.Key.minusculos,
                                moscas = trampa.Key.moscas,
                                mosquitas = trampa.Key.mosquitas,
                                polillas = trampa.Key.polillas,
                                roedor = trampa.Key.roedor,
                                insecto = trampa.Key.insecto,
                                cucaAmericana = trampa.Key.cucaAmericana,
                                cucaGermanica = trampa.Key.cucaGermanica,
                                idAccion = trampa.Key.idAccion,
                                idEstado = trampa.Key.idEstado,
                                descripcionAccion = trampa.Key.descripcionAccion,
                                descripcionEstado = trampa.Key.descripcionEstado,
                                descripcionAccionAbr = trampa.Key.descripcionAccionAbr,
                                descripcionEstadoAbr = trampa.Key.descripcionEstadoAbr,
                                is_cargado = trampa.Key.is_cargado,
                                observaciones = trampa.Key.observaciones,
                                ordenNumero = getIntFromString(trampa.Key.numero),
                                ordenSubNumero = getSubNumero(trampa.Key.numero)
                            }).OrderBy(o => o.ordenNumero).ThenBy(t => t.ordenSubNumero).ToList()
                    }
                );

            return trampasGroup.ToList();

        }
        public List<Trampas_OrdenTrabajoDetalle> getTrampasDetalleByidOrdenTrabajo(int idOrdenTrabajo)
        {

            FoodDefense_DevEntities db = new FoodDefense_DevEntities();

            List<Trampas_OrdenTrabajoDetalle> result = (from otd in db.ORDEN_TRABAJO_DETALLE
                                                        join tcls in db.TRAMPA_CLIENTE_LOCACION_SECTOR on otd.idTrampaClienteLocacionSector equals tcls.id
                                                        join t in db.TRAMPA on tcls.idTrampa equals t.id
                                                        join tt in db.TRAMPA_TIPO on t.idTrampaTipo equals tt.id
                                                        join cls in db.CLIENTE_LOCACION_SECTOR on tcls.idClienteLocacionSector equals cls.id
                                                        // join tce in db.TRAMPA_CONTROL_ESTADO on otd.idEstado equals tce.id into otd_tce
                                                        // from tce in otd_tce.DefaultIfEmpty()
                                                        join tca in db.TRAMPA_CONTROL_ACCION on otd.idAccion equals tca.id into otd_tca
                                                        from tca in otd_tca.DefaultIfEmpty()
                                                        where otd.idOrdenTrabajo == idOrdenTrabajo
                                                        select new Trampas_OrdenTrabajoDetalle
                                                        {
                                                            cargado = (bool)otd.cargado,
                                                            descripcion = cls.descripcion + " / " + t.numero,
                                                            descTipoTrampa = tt.descripcion,
                                                            idAccion = otd.idAccion,
                                                            //idEstado = otd.idEstado,
                                                            idOrdenTrabajo = otd.idOrdenTrabajo,
                                                            idOrdenTrabajoDetalle = otd.id,
                                                            idTipoTrampa = tt.id,
                                                            idTrampaClienteLocacionSector = otd.idTrampaClienteLocacionSector,
                                                            mariposas = (int)otd.mariposas,
                                                            minusculos = (int)otd.minusculos,
                                                            moscas = (int)otd.moscas,
                                                            mosquitas = (int)otd.mosquitas,
                                                            polillas = (int)otd.polillas,
                                                            roedor = otd.roedor,
                                                            insecto = otd.insecto,
                                                            cucaAmericana = otd.cucaAmericana,
                                                            cucaGermanica = otd.cucaGermanica,
                                                            cantidad = (int)otd.cantidad,
                                                            numero = t.numero,
                                                            observaciones = otd.observaciones == null ? "" : otd.observaciones,
                                                            descripcionAccion = tca == null ? "" : tca.descripcion,
                                                            //descripcionEstado = tce == null ? "" : tce.descripcion,
                                                            descripcionAccionAbr = tca == null ? "" : tca.abreviatura,
                                                            //descripcionEstadoAbr = tce == null ? "" : tce.abreviatura,
                                                            is_cargado = otd.cargado == true ? "S" : "N"
                                                        }).ToList();

            return result;
        }

        public List<TrampaControlDetallaAbreviaciones> getTrampaControlDetallaAbreviaciones()
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();

            List<TrampaControlDetallaAbreviaciones> resultTCE = new List<TrampaControlDetallaAbreviaciones>();
            resultTCE = (from tce in db.TRAMPA_CONTROL_ESTADO
                         join tt in db.TRAMPA_TIPO on tce.idTrampaTipo equals tt.id
                         where tt.baja == false
                         select new TrampaControlDetallaAbreviaciones
                         {
                             abreviatura = tce.abreviatura,
                             descripcion = tce.descripcion,
                             idTrampaTipo = tce.idTrampaTipo,
                             trampaTipoDesc = tt.descripcion,
                             tipo = "E",
                             orden = (byte)tce.orden
                         }
            ).OrderBy(o => o.idTrampaTipo).ThenBy(o => o.orden).ToList();

            List<TrampaControlDetallaAbreviaciones> resultTCA = new List<TrampaControlDetallaAbreviaciones>();
            resultTCA = (from tca in db.TRAMPA_CONTROL_ACCION
                         join tt in db.TRAMPA_TIPO on tca.idTrampaTipo equals tt.id
                         where tt.baja == false
                         select new TrampaControlDetallaAbreviaciones
                         {
                             abreviatura = tca.abreviatura,
                             descripcion = tca.descripcion,
                             idTrampaTipo = tca.idTrampaTipo,
                             trampaTipoDesc = tt.descripcion,
                             tipo = "A",
                             orden = (byte)tca.orden
                         }
                      ).OrderBy(o => o.idTrampaTipo).ThenBy(o => o.orden).ToList();

            List<TrampaControlDetallaAbreviaciones> result = new List<TrampaControlDetallaAbreviaciones>();
            result.AddRange(resultTCE);
            result.AddRange(resultTCA);

            return result;

        }

        #region carga de trabajo
        public List<string> getCampoRequeridoCargaControlTrampa(int idTrampaTipo)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            return db.CAMPO_REQUERIDO_CARGA_CONTROL_TRAMPA.Where(w => w.idTrampaTipo == idTrampaTipo).Select(s => s.campo).ToList();
        }
        public List<TRAMPA_CONTROL_ACCION> getTrampaControlAccion(int idTrampaTipo)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            return db.TRAMPA_CONTROL_ACCION.Where(w => w.idTrampaTipo == idTrampaTipo).OrderBy(o => o.orden).ToList();
        }
        public List<TRAMPA_CONTROL_ESTADO> getTrampaControlEstado(int idTrampaTipo)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            return db.TRAMPA_CONTROL_ESTADO.Where(w => w.idTrampaTipo == idTrampaTipo).OrderBy(o => o.orden).ToList();
        }
        public TRAMPA_CONTROL_ESTADO getTrampaControlEstadoById(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            return db.TRAMPA_CONTROL_ESTADO.Find(id);
        }
        public TRAMPA_CONTROL_ACCION getTrampaControlAccionById(int id)
        {
            FoodDefense_DevEntities db = new FoodDefense_DevEntities();
            return db.TRAMPA_CONTROL_ACCION.Find(id);
        }

        public ResultOrdenTrabajoDetalle saveTrabajo(Trampas_OrdenTrabajoDetalle trampas_OrdenTrabajoDetalle)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                try
                {
                    ORDEN_TRABAJO_DETALLE orden_trabajo_detalle = db.ORDEN_TRABAJO_DETALLE.Find(trampas_OrdenTrabajoDetalle.idOrdenTrabajoDetalle);

                    if (trampas_OrdenTrabajoDetalle.is_cantidad == "S") orden_trabajo_detalle.cantidad = trampas_OrdenTrabajoDetalle.cantidad;
                    if (trampas_OrdenTrabajoDetalle.is_idAccion == "S") orden_trabajo_detalle.idAccion = trampas_OrdenTrabajoDetalle.idAccion;
                    //if (trampas_OrdenTrabajoDetalle.is_idEstado == "S") orden_trabajo_detalle.idEstado = trampas_OrdenTrabajoDetalle.idEstado;
                    if (trampas_OrdenTrabajoDetalle.is_mariposas == "S") orden_trabajo_detalle.mariposas = trampas_OrdenTrabajoDetalle.mariposas;
                    if (trampas_OrdenTrabajoDetalle.is_minusculos == "S") orden_trabajo_detalle.minusculos = trampas_OrdenTrabajoDetalle.minusculos;
                    if (trampas_OrdenTrabajoDetalle.is_moscas == "S") orden_trabajo_detalle.moscas = trampas_OrdenTrabajoDetalle.moscas;
                    if (trampas_OrdenTrabajoDetalle.is_mosquitas == "S") orden_trabajo_detalle.mosquitas = trampas_OrdenTrabajoDetalle.mosquitas;
                    if (trampas_OrdenTrabajoDetalle.is_polillas == "S") orden_trabajo_detalle.polillas = trampas_OrdenTrabajoDetalle.polillas;
                    if (trampas_OrdenTrabajoDetalle.is_roedor == "S") orden_trabajo_detalle.roedor = trampas_OrdenTrabajoDetalle.roedor;
                    if (trampas_OrdenTrabajoDetalle.is_insecto == "S") orden_trabajo_detalle.insecto = trampas_OrdenTrabajoDetalle.insecto;
                    if (trampas_OrdenTrabajoDetalle.is_cucaAmericana == "S") orden_trabajo_detalle.cucaAmericana = trampas_OrdenTrabajoDetalle.cucaAmericana;
                    if (trampas_OrdenTrabajoDetalle.is_cucaGermanica == "S") orden_trabajo_detalle.cucaGermanica = trampas_OrdenTrabajoDetalle.cucaGermanica;

                    orden_trabajo_detalle.observaciones = trampas_OrdenTrabajoDetalle.observaciones;
                    orden_trabajo_detalle.cargado = true;

                    db.Entry(orden_trabajo_detalle).State = EntityState.Modified;
                    db.SaveChanges();

                    return new ResultOrdenTrabajoDetalle
                    {
                        orden_trabajo_detalle = orden_trabajo_detalle,
                        errMsg = "",
                        success = true
                    };
                }
                catch (Exception e)
                {
                    return new ResultOrdenTrabajoDetalle
                    {
                        orden_trabajo_detalle = null,
                        errMsg = e.Message,
                        success = false
                    };
                }

            }
        }
        public ResultOrdenTrabajoDetalle restartTrabajo(int id)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                try
                {
                    ORDEN_TRABAJO_DETALLE orden_trabajo_detalle = db.ORDEN_TRABAJO_DETALLE.Find(id);

                    orden_trabajo_detalle.cantidad = null;
                    orden_trabajo_detalle.idAccion = null;
                    //orden_trabajo_detalle.idEstado = null;
                    orden_trabajo_detalle.mariposas = null;
                    orden_trabajo_detalle.minusculos = null;
                    orden_trabajo_detalle.moscas = null;
                    orden_trabajo_detalle.mosquitas = null;
                    orden_trabajo_detalle.polillas = null;
                    orden_trabajo_detalle.roedor = null;
                    orden_trabajo_detalle.insecto = null;
                    orden_trabajo_detalle.cucaAmericana = null;
                    orden_trabajo_detalle.cucaGermanica = null;
                    orden_trabajo_detalle.observaciones = null;
                    orden_trabajo_detalle.cargado = false;


                    db.Entry(orden_trabajo_detalle).State = EntityState.Modified;
                    db.SaveChanges();

                    return new ResultOrdenTrabajoDetalle
                    {
                        orden_trabajo_detalle = orden_trabajo_detalle,
                        errMsg = "",
                        success = true
                    };
                }
                catch (Exception e)
                {
                    return new ResultOrdenTrabajoDetalle
                    {
                        orden_trabajo_detalle = null,
                        errMsg = e.Message,
                        success = false
                    };
                }

            }
        }
        public ResultOrdenTrabajo finalizeTrabajo(int id)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                try
                {
                    ORDEN_TRABAJO orden_trabajo = db.ORDEN_TRABAJO.Find(id);

                    orden_trabajo.idOrdenTrabajoEstado = 3;

                    db.Entry(orden_trabajo).State = EntityState.Modified;
                    db.SaveChanges();

                    return new ResultOrdenTrabajo
                    {
                        orden_trabajo = orden_trabajo,
                        errMsg = "",
                        success = true
                    };
                }
                catch (Exception e)
                {
                    return new ResultOrdenTrabajo
                    {
                        orden_trabajo = null,
                        errMsg = e.Message,
                        success = false
                    };
                }

            }
        }

        public ResultOrdenTrabajo saveTrabajoEstados(int idOrdenTrabajoDetalle, List<int> listEstado )
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                try
                {
                    List<ORDEN_TRABAJO_DETALLE_ESTADO> borrarTrabajoEstados;
                    borrarTrabajoEstados  = db.ORDEN_TRABAJO_DETALLE_ESTADO.Where(n => n.idOrdenTrabajoDetalle == idOrdenTrabajoDetalle).ToList();
                    db.ORDEN_TRABAJO_DETALLE_ESTADO.RemoveRange(borrarTrabajoEstados);
                    db.SaveChanges();

                    foreach (var t in listEstado)
                    {
                        ORDEN_TRABAJO_DETALLE_ESTADO oTDESTADO = new ORDEN_TRABAJO_DETALLE_ESTADO();
                        oTDESTADO.idOrdenTrabajoDetalle = idOrdenTrabajoDetalle;
                        oTDESTADO.idEstado = t;
                        db.ORDEN_TRABAJO_DETALLE_ESTADO.Add(oTDESTADO);
                        db.SaveChanges();
                    }
                    return new ResultOrdenTrabajo
                    {
                        orden_trabajo = null,
                        errMsg = "",
                        success = true
                    };
                }
                catch (Exception e)
                {
                    return new ResultOrdenTrabajo
                    {
                        orden_trabajo = null,
                        errMsg = e.Message,
                        success = false
                    };
                }

            }
        }

        public bool isTrabajoTodoCargado(int idOrdenTrabajo)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                List<ORDEN_TRABAJO_DETALLE> oRDEN_TRABAJO_DETALLE = db.ORDEN_TRABAJO_DETALLE.Where(a => a.idOrdenTrabajo == idOrdenTrabajo).ToList();
                return !oRDEN_TRABAJO_DETALLE.Any(a => a.cargado == false);
            }
        }
        #endregion

        #region appMovil
        public List<validarTrampa_Result> validarTrampa(int idTrampaClienteLocacionSector, int idEmpleado)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                return db.st_validarTrampaCargaApp(idTrampaClienteLocacionSector, idEmpleado).ToList();
            }
        }

        public Trampas_OrdenTrabajoDetalle armarDatosYCampoRequerido(int id)
        {

            Trampas_OrdenTrabajoDetalle trampas_OrdenTrabajo = new Trampas_OrdenTrabajoDetalle();

            trampas_OrdenTrabajo = getTrampaDetalle(id);
            //activar controles
            List<string> controles = getCampoRequeridoCargaControlTrampa(trampas_OrdenTrabajo.idTipoTrampa);

            //idEstado
            if (controles.Any(s => s.Contains("idEstado")))
            {
                trampas_OrdenTrabajo.is_idEstado = "S";
                trampas_OrdenTrabajo.selectEstado = (from p in getTrampaControlEstado(trampas_OrdenTrabajo.idTipoTrampa)
                                                     select new Select
                                                     {
                                                         id = p.id,
                                                         valor = p.descripcion + '(' + p.abreviatura + ')'
                                                     }
                                      ).ToList();

            }
            //idAccion
            if (controles.Any(s => s.Contains("idAccion")))
            {
                trampas_OrdenTrabajo.is_idAccion = "S";
                trampas_OrdenTrabajo.selectAccion = (from p in getTrampaControlAccion(trampas_OrdenTrabajo.idTipoTrampa)
                                                     select new Select
                                                     {
                                                         id = p.id,
                                                         valor = p.descripcion + '(' + p.abreviatura + ')'
                                                     }
                                  ).ToList();

            }

            //moscas
            if (controles.Any(s => s.Contains("moscas")))
            {
                trampas_OrdenTrabajo.is_moscas = "S";
            }
            //mosquitas
            if (controles.Any(s => s.Contains("mosquitas")))
            {
                trampas_OrdenTrabajo.is_mosquitas = "S";
            }
            //polillas
            if (controles.Any(s => s.Contains("polillas")))
            {
                trampas_OrdenTrabajo.is_polillas = "S";
            }
            //mariposas
            if (controles.Any(s => s.Contains("mariposas")))
            {
                trampas_OrdenTrabajo.is_mariposas = "S";
            }
            //minusculos
            if (controles.Any(s => s.Contains("minusculos")))
            {
                trampas_OrdenTrabajo.is_minusculos = "S";
            }
            //roedor
            if (controles.Any(s => s.Contains("roedor")))
            {
                trampas_OrdenTrabajo.is_roedor = "S";
            }
            //insecto
            if (controles.Any(s => s.Contains("insecto")))
            {
                trampas_OrdenTrabajo.is_insecto = "S";
            }
            //cucaGermanica
            if (controles.Any(s => s.Contains("cucaGermanica")))
            {
                trampas_OrdenTrabajo.is_cucaGermanica = "S";
            }
            //cucaAmericana
            if (controles.Any(s => s.Contains("cucaAmericana")))
            {
                trampas_OrdenTrabajo.is_cucaAmericana = "S";
            }
            //cantidad
            if (controles.Any(s => s.Contains("cantidad")))
            {
                trampas_OrdenTrabajo.is_cantidad = "S";
            }

            trampas_OrdenTrabajo.listEstado = getEstadosDetalleSeleccionados(trampas_OrdenTrabajo.idOrdenTrabajoDetalle);

            return trampas_OrdenTrabajo;

        }

        public  List<string> getLastTrap(int idOrdenTrabajoDetalle)
        {
            
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                try
                {
                    List<LastTrampaApp_Result> lastTrampaApp_Result = new List<LastTrampaApp_Result>();
                   lastTrampaApp_Result = db.st_getLastTrampaApp(idOrdenTrabajoDetalle).ToList();
                    if (lastTrampaApp_Result != null)
                    {
                        if (lastTrampaApp_Result.Count > 0) {                            
                            List<string> lstLastTrap = new List<string>();
                            
                            foreach (var item in lastTrampaApp_Result)
                            {
                                lstLastTrap.Add(item.desctrampatipo + "=> Número: " + item.OrdenNumero.ToString() + item.OrdenSubNumero.ToString());
                            }
                            return lstLastTrap;
                        }
                        return new List<string>(); 
                    }
                    else {
                        return new List<string>(); 
                    }
                }
                catch (Exception)
                {
                    return new List<string>(); 
                }
              
            }
        }

        #endregion

        public List<AutocompleteTrampas> autocompleteTrampas(string numero, int idTrampaTipo)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                return db.st_AutocompleteTrampas(numero, idTrampaTipo).ToList();
            }
        }

        public List<Select> getClienteSelect()
        {
            return (from p in getCliente()
                   select new Select
                   {
                       id = p.id,
                       valor = p.razonSocial
                   }
                   ).ToList();            
        }

        public List<CLIENTE> getCliente()
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                return db.CLIENTE.Where(w => w.baja == false).ToList();
            }
        }

        public List<SelectChild> getLocacionesSelect()
        {
            return (from p in getLocaciones()
                    select new SelectChild
                    {
                        id = p.id,
                        valor = p.descripcion,
                        idMaster = p.idCliente
                    }
                   ).ToList();
        }

        public List<int> getTrampaApp(int idClienteLocacion, int idTrampaTipo,string numeroTrampa, int idEmpleado)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                return (from p in db.st_getTrampaApp(idClienteLocacion,idTrampaTipo,numeroTrampa,idEmpleado)
                        select                     
                            p.TrampaClienteLocacionSector                    
                       ).ToList();
            }            
        }

        public List<SelectChild> getLocacionesSelectByIdCliente(int idCliente)
        {
            return (from p in getLocacionesByIdCliente(idCliente)
                    select new SelectChild
                    {
                        id = p.id,
                        valor = p.descripcion,
                        idMaster = p.idCliente
                    }
                   ).ToList();
        }

        public List<CLIENTE_LOCACION> getLocaciones()
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                return  db.CLIENTE_LOCACION.Where(n => n.baja == false).ToList();
            }
        }

        public List<CLIENTE_LOCACION> getLocacionesByIdCliente(int idCliente)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                return db.CLIENTE_LOCACION.Where(n => n.baja == false && n.idCliente == idCliente).ToList();
            }
        }

        public List<Select> getTrampatipoSelect()
        {
            return (from p in getTrampaTipo()
                    select new Select
                    {
                        id = p.id,
                        valor = p.descripcion
                    }
                   ).ToList();
        }

        public List<TRAMPA_TIPO> getTrampaTipo()
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                return db.TRAMPA_TIPO.Where(w => w.baja == false).ToList();
            }
        }

        public List<int> getEstadosDetalleSeleccionados(int idDetalle)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                List<int>lst = (from otde in db.ORDEN_TRABAJO_DETALLE_ESTADO                           
                           where otde.idOrdenTrabajoDetalle == idDetalle
                           select otde.idEstado).ToList();

                return lst;
                // return db.ORDEN_TRABAJO_DETALLE_ESTADO.Where(w => w.idOrdenTrabajoDetalle == idDetalle);
            }
        }

        public string controlCantidadRequerida(ControlTrampaRequest c) {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                int idTrampaClienteLocacionSector = db.ORDEN_TRABAJO_DETALLE.Where(w => w.id == c.idOrdenTrabajoDetalle).Select(s => s.idTrampaClienteLocacionSector).First();
                return db.st_control_cantidad_requerida_estado_trampa(idTrampaClienteLocacionSector,c.idAccion,1,c.moscas,c.mosquitas,
                    c.polillas, c.mariposas, c.minusculos, c.cantidad, c.roedor, c.insecto, c.cucaGermanica, c.cucaAmericana,c.observaciones).First();
            }
        }

        #region utils
        private int getIntFromString(string subjectString)
        {
            string resultString = Regex.Match(subjectString, @"\d+").Value;
            if (String.IsNullOrEmpty(resultString.Trim()))
                return 0;
            else
                return Int32.Parse(resultString);
        }

        private string getSubNumero(string subjectString)
        {
            char resultString = subjectString.Trim().Last();
            if (Char.IsNumber(resultString))
                return "0";
            else
                return resultString.ToString();
        }
        #endregion

        public List<WorkListTareasData> GetListAccionesTareas(int idOrdenTrabajo, int idTipoTrampa, int idOrdenTrabajoDetalle)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                return (from p in db.st_GetListAccionesTareas (idOrdenTrabajo, idTipoTrampa, idOrdenTrabajoDetalle)
                        select new WorkListTareasData
                        {
                            idTarea = (int)p.id,
                            descripcion = p.descripcion,
                            realizado = (int)p.realizado,
                        }
                ).ToList();               
            }
        }

        public bool ListAccionesTareasInsert(WorkListTareasInsert lst)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                List<ORDEN_TRABAJO_DETALLE_CONTROL_PLAGA> borrarOTDCP = 
                    db.ORDEN_TRABAJO_DETALLE_CONTROL_PLAGA.Where(n => n.idOrdenTrabajoDetalle == lst.idOrdenTrabajoDetalle).ToList();

                db.ORDEN_TRABAJO_DETALLE_CONTROL_PLAGA.RemoveRange(borrarOTDCP);
                db.SaveChanges();

                foreach (var t in lst.listAcciones)
                {
                    ORDEN_TRABAJO_DETALLE_CONTROL_PLAGA oTDCP = new ORDEN_TRABAJO_DETALLE_CONTROL_PLAGA();
                    oTDCP.idOrdenTrabajoDetalle = lst.idOrdenTrabajoDetalle;
                    oTDCP.idTarea = t.idTarea;
                    oTDCP.realizado = Convert.ToBoolean(t.realizado);
                    db.ORDEN_TRABAJO_DETALLE_CONTROL_PLAGA.Add(oTDCP);
                    db.SaveChanges();
                }
                return true;
            }
        }
           
}
}