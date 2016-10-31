﻿using ClinicaFrba.Clases.POJOS;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TostadoPersistentKit;

namespace ClinicaFrba.Clases.DAOS
{
    public class AfiliadoRepository : Repository
    {
        internal override Type getModelClassType()
        {
            return typeof(Afiliado);
        }

        public string insertarAfiliado(Afiliado afiliado)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            long nroRaiz = afiliado.numeroDeAfiliado - afiliado.numeroFamiliar;
            nroRaiz = nroRaiz < 0 ? 0 : nroRaiz;

            DataBase.Instance.agregarParametro(parametros, "nro_raiz",nroRaiz );
            DataBase.Instance.agregarParametro(parametros, "nro_grupo_familiar", afiliado.numeroFamiliar);

            autoMapping = false;

            Dictionary<string, object> result = ((List<Dictionary<string, object>>)
                                                executeStored("BEMVINDO.st_insertar_afiliado",
                                                afiliado, parametros))[0];

            autoMapping = true;

            afiliado.usuario.nick = result["nick"].ToString();
            afiliado.usuario.pass = result["pass"].ToString();
            afiliado.numeroDeAfiliado = Convert.ToInt64(result["id_afiliado"]);//Seteo las nuevas propiedades

            return result["error"].ToString();
        }

        internal Afiliado traerAfiliadoPorUser(Usuario usuario)
        {
            List<Afiliado> afiliados = (List<Afiliado>)selectByProperty("usuario", usuario.id);

            return afiliados.Count > 0 ? afiliados[0] : null;
        }

        internal void darDeBajaAfiliado(Afiliado afiliado)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            DataBase.Instance.agregarParametro(parametros, "id_afiliado", afiliado.usuario.id);
            DataBase.Instance.agregarParametro(parametros, "fecha_baja", DataBase.Instance.getDate());

            executeStored("BEMVINDO.st_baja_afiliado", parametros);
        }

        internal List<Afiliado> buscarAfiliados(long nroAfiliado, string nombre, string apellido, string dni, PlanMedico planMedico)
        {
            object nroAfiliadoValue = nroAfiliado == 0 ? null : (object)nroAfiliado;
            object nombreValue = nombre == "" ? null : nombre;
            object apellidoValue = apellido == "" ? null : apellido;
            object dniValue = dni == "" ? null : dni;
            object planValue = planMedico.id==0 ? null : (object)planMedico.id;

            List<SqlParameter> parametros = new List<SqlParameter>();
            DataBase.Instance.agregarParametro(parametros, "nroAfiliado", nroAfiliadoValue);
            DataBase.Instance.agregarParametro(parametros, "nombre", nombreValue);
            DataBase.Instance.agregarParametro(parametros, "apellido", apellidoValue);
            DataBase.Instance.agregarParametro(parametros, "dni", dniValue);
            DataBase.Instance.agregarParametro(parametros, "planMedico", planValue);

            return (List<Afiliado>)executeStored("BEMVINDO.st_buscar_afiliados", parametros);
        }

        internal void modificarAfiliado(Afiliado afiliado,string motivo)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            DataBase.Instance.agregarParametro(parametros, "motivo", motivo);
            DataBase.Instance.agregarParametro(parametros, "fecha_sistema", DataBase.Instance.getDate());

            executeStored("BEMVINDO.st_actualizar_afiliado", afiliado, parametros);
        }

        internal Afiliado traerAfiliadoPorId(long id)
        {
            return (Afiliado)selectById(id);
        }
    }
}
