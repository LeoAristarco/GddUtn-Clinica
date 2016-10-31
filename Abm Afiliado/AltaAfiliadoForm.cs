﻿using ClinicaFrba.Clases.Otros;
using ClinicaFrba.Clases.POJOS;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ClinicaFrba.Abm_Afiliado
{
    public partial class AltaAfiliadoForm : Form
    {
        private AltaAfiliado altaAfiliado = new AltaAfiliado();

        public bool altaExitosa = false;
        public bool altaConyuge = false;
        public bool altaHijo = false;

        public AltaAfiliadoForm()
        {
            InitializeComponent();
        }

        public AltaAfiliadoForm(Afiliado familiar)
        {
            altaAfiliado.nuevoAfiliado = familiar;

            InitializeComponent();
        }

        private void AltaAfiliadoForm_Load(object sender, EventArgs e)
        {
            initForm();
        }

        private void initForm()
        {
            txtNombre.DataBindings.Add("Text", altaAfiliado.nuevoAfiliado.usuario, "nombre");
            txtApellido.DataBindings.Add("Text", altaAfiliado.nuevoAfiliado.usuario, "apellido");
            dtpFechaNacimiento.DataBindings.Add("Value", altaAfiliado.nuevoAfiliado.usuario, "fechaDeNacimiento");
            txtDni.DataBindings.Add("Text", altaAfiliado.nuevoAfiliado.usuario, "documento");
            txtDir.DataBindings.Add("Text", altaAfiliado.nuevoAfiliado.usuario, "direccion");
            txtTel.DataBindings.Add("Text", altaAfiliado.nuevoAfiliado.usuario, "telefono");
            txtMail.DataBindings.Add("Text", altaAfiliado.nuevoAfiliado.usuario, "mail");
            txtHijos.DataBindings.Add("Text", altaAfiliado.nuevoAfiliado, "cantidadDeHijos");

            cmbEstadoCivil.DisplayMember = "descripcion";
            cmbEstadoCivil.DataSource = altaAfiliado.estadosCivilesSistema;
            cmbEstadoCivil.DataBindings.Add("SelectedItem", altaAfiliado.nuevoAfiliado, "estadoCivil");

            cmbSexo.DataSource = altaAfiliado.sexosSistema;
            cmbSexo.DataBindings.Add("SelectedItem", altaAfiliado.nuevoAfiliado.usuario, "sexo");

            cmbTipoDocumento.DisplayMember = "descripcion";
            cmbTipoDocumento.DataSource = altaAfiliado.tiposDeDocumentoSistema;
            cmbTipoDocumento.DataBindings.Add("SelectedItem", altaAfiliado.nuevoAfiliado.usuario, "tipoDeDocumento");

            cmbPlanes.DisplayMember = "descripcion";
            cmbPlanes.DataSource = altaAfiliado.planesMedicosSistema;
            cmbPlanes.DataBindings.Add("SelectedItem", altaAfiliado.nuevoAfiliado, "planMedico");

            if (altaConyuge)
            {
                cmbEstadoCivil.Enabled = false;
                txtHijos.Enabled = false;
                txtDir.Enabled = false;                            
            }

            if (altaFamiliar())
            {
                cmbPlanes.Enabled = false;
                btnConyuge.Enabled = false;
                btnHijo.Enabled = false;
            }
        }

        private void cmbVolver_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmdLimpiar_Click(object sender, EventArgs e)
        {
        }

        private void cmdAceptar_Click(object sender, EventArgs e)
        {
            if (!altaAfiliado.cumpleValidaciones())
            {
                MessageBox.Show(altaAfiliado.mensajeDeError);
                return;
            }

            altaExitosa = true;

            if (!altaFamiliar())
            {
                if (!altaAfiliado.guardarAfiliado())
                {
                    MessageBox.Show(altaAfiliado.mensajeDeError);
                    return;
                }
                
                MessageBox.Show("Afiliado creado exitosamente");

                Hide();

                (new MostrarAfiliadosCreadosForm(altaAfiliado.nuevoAfiliado)).ShowDialog();
            }

            Close();
        }

        private bool altaFamiliar()
        {
            return altaHijo || altaConyuge;
        }

        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void txtTel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void cmbEstadoCivil_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEstadoCivil.SelectedItem!=null)
            {
                altaAfiliado.nuevoAfiliado.estadoCivil = (EstadoCivil)cmbEstadoCivil.SelectedItem;

                btnConyuge.Enabled = altaAfiliado.afiliadoTieneConyuge();
            }
        }

        private void btnConyuge_Click(object sender, EventArgs e)
        {
            if (!altaAfiliado.cumpleValidaciones())
            {
                MessageBox.Show(altaAfiliado.mensajeDeError);
                return;
            }

            altaAfiliado.crearConyuge();

            AltaAfiliadoForm altaConyuge = new AltaAfiliadoForm(altaAfiliado.nuevoAfiliado.conyuge);

            altaConyuge.altaConyuge = true;

            Hide();

            altaConyuge.ShowDialog();

            Show();

            if (!altaConyuge.altaExitosa)
            {
                altaAfiliado.nuevoAfiliado.conyuge = null;
            }
        }

        private void txtHijos_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void txtHijos_TextChanged(object sender, EventArgs e)
        {
            long cantHijos = txtHijos.Text == "" ? 0 : Convert.ToInt64(txtHijos.Text);
            btnHijo.Enabled = cantHijos > 0;
        }

        private void btnHijo_Click(object sender, EventArgs e)
        {
            if (!altaAfiliado.cumpleValidaciones())
            {
                MessageBox.Show(altaAfiliado.mensajeDeError);
                return;
            }

            Afiliado hijoAfiliado = altaAfiliado.crearNuevoHijo();

            if (hijoAfiliado==null)
            {
                MessageBox.Show(altaAfiliado.mensajeDeError);
                return;
            }

            AltaAfiliadoForm altaHijo = new AltaAfiliadoForm(hijoAfiliado);
            altaHijo.altaHijo = true;

            Hide();

            altaHijo.ShowDialog();

            Show();

            if (!altaHijo.altaExitosa)
            {
                altaAfiliado.borrarHijo(hijoAfiliado);
            }
        }
    }
}
