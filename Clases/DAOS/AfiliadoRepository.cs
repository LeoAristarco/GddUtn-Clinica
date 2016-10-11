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

            DataBase.Instance.agregarParametro(parametros, "error", "");

            autoMapping = false;

            List <Dictionary < string,object>> result = (List<Dictionary<string, object>>)
                                                executeStored("BEMVINDO.st_insertar_afiliado", 
                                                afiliado, parametros);

            autoMapping = true;

            return result[0]["error"].ToString();
        }
    }
}
