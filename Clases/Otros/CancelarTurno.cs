﻿using ClinicaFrba.Clases.DAOS;
using ClinicaFrba.Clases.POJOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TostadoPersistentKit;

namespace ClinicaFrba.Clases.Otros
{
    class CancelarTurno
    {
        public TipoCancelacion tipoDeCancelacion { get; set; }
        public string motivoDeCancelacion { get; set; }
        public Afiliado afiliado { get; set; }
        public string mensajeDeError { get; set; }
        public List<TipoCancelacion> tiposDeCancelacion { get; set; }
        public List<Turno> turnosDeAfiliado { get; set; }
        public Turno turnoACancelar { get; set; }
        private TurnoRepository repoTurno = new TurnoRepository();

        public CancelarTurno()
        {
            mensajeDeError = "";
            motivoDeCancelacion = "";

            //inicializarListas();
        }

        public void inicializarListas()
        {
            tiposDeCancelacion = repoTurno.traerTiposDeCancelacion();
            turnosDeAfiliado = repoTurno.traerTurnosDeAfiliado(afiliado).Where(t => t.fechaDeTurno.Date >= DataBase.Instance.getDate().Date).ToList();
        }

        internal bool cancelacionExitosa()
        {
            if (!cumpleValidaciones())
            {
                return false;
            }

            ejecutarCancelacion();
            return true;
        }

        private void ejecutarCancelacion()
        {
            repoTurno.cancelarTurno(turnoACancelar, motivoDeCancelacion, tipoDeCancelacion);
        }

        private bool cumpleValidaciones()
        {
            if (motivoDeCancelacion == "")
            {
                mensajeDeError = "Debe completar el motivo de cancelacion";
                return false;
            }
            if (turnoACancelar==null)
            {
                mensajeDeError = "Debe seleccionar un turno a cancelar";
                return false;
            }
            if (turnoYaPaso())
            {
                mensajeDeError = "No se puede cancelar un turno en el que ya ocurrio la fecha pactada";
                return false;
            }
            if (turnoEsDeHoy())
            {
                mensajeDeError = "Se necesita al menos 1 dia de antelacion para cancelar un turno";
                return false;
            }
            if (tipoDeCancelacion == null)
            {
                mensajeDeError = "Debe seleccionar un tipo de cancelacion";
                return false;
            }

            return true;
        }

        private bool turnoYaPaso()
        {
            return DataBase.Instance.getDate() > turnoACancelar.fechaDeTurno;
        }

        public bool turnoEsDeHoy()
        {
            DateTime fechaActual = DataBase.Instance.getDate();
            return (turnoACancelar.fechaDeTurno - fechaActual).Days == 0;
        }
    }
}
