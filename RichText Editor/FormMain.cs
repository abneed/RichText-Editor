// RichText Editor
// Copyright © 2018 Guillermo A. Rodríguez
//
// Licencia MIT(https://es.wikipedia.org/wiki/Licencia_MIT)
//
// Se concede permiso, libre de cargos, a cualquier persona que obtenga una copia
// de este software y de los archivos de documentación asociados (el "Software"), 
// para utilizar el Software sin restricción, incluyendo sin limitación los derechos
// a usar, copiar, modificar, fusionar, publicar, distribuir, sublicenciar, y/o vender copias
// del Software, y a permitir a las personas a las que se les proporcione el Software a hacer
// lo mismo, sujeto a las siguientes condiciones:
//
// El aviso de copyright anterior y este aviso de permiso se incluirán en todas
// las copias o partes sustanciales del Software.
//
// EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O IMPLÍCITA, 
// INCLUYENDO PERO NO LIMITADA A GARANTÍAS DE COMERCIALIZACIÓN, IDONEIDAD PARA UN PROPÓSITO PARTICULAR
// Y NO INFRACCIÓN.EN NINGÚN CASO LOS AUTORES O PROPIETARIOS DE LOS DERECHOS DE AUTOR SERÁN RESPONSABLES
// DE NINGUNA RECLAMACIÓN, DAÑOS U OTRAS RESPONSABILIDADES, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO
// O CUALQUIER OTRO MOTIVO, DERIVADAS DE, FUERA DE O EN CONEXIÓN CON EL SOFTWARE O SU USO U OTRO TIPO
// DE ACCIONES EN EL SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RichText_Editor
{
    public partial class FormMain : Form
    {
        #region Constantes

        private const string NombreFuentePredeterminada = "Calibri";
        private const int TamanoFuentePredeterminada = 15;

        #endregion Constantes

        #region Miembros Privados

        private int m_intConteoPestanas = 0;
        private Font m_fontFuenteSeleccionada;

        #endregion

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.m_fontFuenteSeleccionada = ObtenerFuentePredeterminado();
            tabControlMain.ContextMenuStrip = contextMenuStripPestana;

            AgregarPestana();
            ObtenerColeccionFuente();
            TamanosFuente();
        }

        #region Propiedades

        public RichTextBox ObtenerDocumentoActual
        {
            get { return (RichTextBox)tabControlMain.SelectedTab.Controls["Cuerpo"]; }
        }

        #endregion Propiedades

        #region Metodos

        #region Pestañas

        private void AgregarPestana()
        {
            RichTextBox Cuerpo = new RichTextBox
            {
                Name = "Cuerpo",
                AcceptsTab = true,
                Dock = DockStyle.Fill,
                ContextMenuStrip = contextMenuStripDocumento,
                Font = this.m_fontFuenteSeleccionada
            };

            this.m_intConteoPestanas++;
            string TextoDocumento = "Documento " + this.m_intConteoPestanas;

            TabPage NuevaPagina = new TabPage
            {
                Name = TextoDocumento,
                Text = TextoDocumento
            };

            NuevaPagina.Controls.Add(Cuerpo);
            tabControlMain.TabPages.Add(NuevaPagina);
        }

        private void AgregarPestana(string Texto)
        {
            RichTextBox Cuerpo = new RichTextBox
            {
                Name = "Cuerpo",
                AcceptsTab = true,
                Dock = DockStyle.Fill,
                ContextMenuStrip = contextMenuStripDocumento,
                Text = Texto,
                Font = this.m_fontFuenteSeleccionada
            };

            this.m_intConteoPestanas++;
            string TextoDocumento = "Documento " + this.m_intConteoPestanas;

            TabPage NuevaPagina = new TabPage
            {
                Name = TextoDocumento,
                Text = TextoDocumento
            };

            NuevaPagina.Controls.Add(Cuerpo);
            tabControlMain.TabPages.Add(NuevaPagina);
        }

    
      

        private void EliminarPestana()
        {
            if (tabControlMain.TabPages.Count != 1)
            {
                tabControlMain.TabPages.Remove(tabControlMain.SelectedTab);
            }
            else
            {
                tabControlMain.TabPages.Remove(tabControlMain.SelectedTab);
                this.m_intConteoPestanas = 0;
                AgregarPestana();
            }
        }

        private void EliminarTodasLasPestanas()
        {
            foreach (TabPage Page in tabControlMain.TabPages)
            {
                tabControlMain.TabPages.Remove(Page);
            }
            this.m_intConteoPestanas = 0;
            AgregarPestana();
        }

        private void EliminarTodasLasPestanasMenosEsta()
        {
            foreach (TabPage Page in tabControlMain.TabPages)
            {
                if (Page.Name != tabControlMain.SelectedTab.Name)
                {
                    tabControlMain.TabPages.Remove(Page);
                }
            }
        }

        #endregion Pestañas

        #region Guardar&Abrir

        private void Abrir()
        {
            openFileDialogDocumento.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialogDocumento.Filter = "Formato de texto enriquecido(RTF) | *.rtf";

            if (openFileDialogDocumento.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (openFileDialogDocumento.FileName.Length > 0)
                {
                    try
                    {
                        AgregarPestana();

                        tabControlMain.SelectedTab = tabControlMain.TabPages["Documento " + this.m_intConteoPestanas];

                        ObtenerDocumentoActual.LoadFile(openFileDialogDocumento.FileName, RichTextBoxStreamType.RichText);
                        string filename = Path.GetFileName(openFileDialogDocumento.FileName);
                        tabControlMain.SelectedTab.Text = filename;
                        tabControlMain.SelectedTab.Name = filename;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }

        private void Guardar()
        {
            saveFileDialogDocumento.FileName = tabControlMain.SelectedTab.Name;
            saveFileDialogDocumento.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialogDocumento.Filter = "Formato de texto enriquecido (RTF)|*.rtf";
            saveFileDialogDocumento.Title = "Guardar";

            if (saveFileDialogDocumento.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (saveFileDialogDocumento.FileName.Length > 0)
                {
                    try
                    {
                        ObtenerDocumentoActual.SaveFile(saveFileDialogDocumento.FileName, RichTextBoxStreamType.RichText);
                        string filename = Path.GetFileName(saveFileDialogDocumento.FileName);
                        tabControlMain.SelectedTab.Text = filename;
                        tabControlMain.SelectedTab.Name = filename;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }

        private void GuardarComo()
        {
            saveFileDialogDocumento.FileName = tabControlMain.SelectedTab.Name;
            saveFileDialogDocumento.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialogDocumento.Filter = "Formato de texto enriquecido (RTF)|*.rtf|Documentos de texto (*.txt)|*.txt";
            saveFileDialogDocumento.Title = "Guardar como";

            if (saveFileDialogDocumento.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (saveFileDialogDocumento.FileName.Length > 0)
                {
                    try
                    {
                        ObtenerDocumentoActual.SaveFile(saveFileDialogDocumento.FileName, RichTextBoxStreamType.RichText);
                        string filename = Path.GetFileName(saveFileDialogDocumento.FileName);
                        tabControlMain.SelectedTab.Text = filename;
                        tabControlMain.SelectedTab.Name = filename;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }

        }

        #endregion Guardar&Abrir

        #region Texto

        private void Deshacer()
        {
            ObtenerDocumentoActual.Undo();
        }

        private void Rehacer()
        {
            ObtenerDocumentoActual.Redo();
        }

        private void Cortar()
        {
            ObtenerDocumentoActual.Cut();
        }

        private void Copiar()
        {
            ObtenerDocumentoActual.Copy();
        }

        private void Pegar()
        {
            ObtenerDocumentoActual.Paste();
        }

        private void SeleccionarTodo()
        {
            ObtenerDocumentoActual.SelectAll();
        }

        private void EstablecerEstiloFuente(FontStyle estiloFuente)
        {

        }

        #endregion Texto

        #region General

        private void ObtenerColeccionFuente()
        {
            InstalledFontCollection FuenteIns = new InstalledFontCollection();

            foreach (FontFamily item in FuenteIns.Families)
            {
                toolStripComboBoxFamiliaFuentes.Items.Add(item.Name);
            }

            toolStripComboBoxFamiliaFuentes.SelectedIndex = toolStripComboBoxFamiliaFuentes.FindString(NombreFuentePredeterminada);
        }

        private void TamanosFuente()
        {
            for (int i = 0; i <= 75; i++)
            {
                toolStripComboBoxTamanoFuente.Items.Add(i);
            }
            toolStripComboBoxTamanoFuente.SelectedIndex = TamanoFuentePredeterminada;
        }

        private Font ObtenerFuentePredeterminado()
        {
            return new Font(NombreFuentePredeterminada, TamanoFuentePredeterminada, FontStyle.Regular);
        }

        #endregion General

        #endregion Metodos

        #region MenuStripPrincipal Eventos

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AgregarPestana();
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Abrir();
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Guardar();
        }

        private void guardarcomoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuardarComo();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void deshacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Deshacer();
        }

        private void rehacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rehacer();
        }

        private void cortarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cortar();
        }

        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copiar();
        }

        private void pegarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pegar();
        }

        private void seleccionartodoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SeleccionarTodo();
        }

        private void acercadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Abrir AcercaDeForm...
        }

        #endregion MenuStripPrincipal Eventos

        #region ToolStripPanelSuperior Eventos

        private void toolStripButtonNegrita_Click(object sender, EventArgs e)
        {
            Font FuenteRegular = new Font(ObtenerDocumentoActual.SelectionFont.FontFamily,
                ObtenerDocumentoActual.SelectionFont.SizeInPoints, FontStyle.Regular);

            Font FuenteNegrita = new Font(ObtenerDocumentoActual.SelectionFont.FontFamily,
                ObtenerDocumentoActual.SelectionFont.SizeInPoints, FontStyle.Bold);

            if (ObtenerDocumentoActual.SelectionFont.Bold)
            {
                ObtenerDocumentoActual.SelectionFont = FuenteRegular;
            }
            else
            {
                ObtenerDocumentoActual.SelectionFont = FuenteNegrita;
            }
        }

        private void toolStripButtonCursiva_Click(object sender, EventArgs e)
        {
            Font FuenteRegular = new Font(ObtenerDocumentoActual.SelectionFont.FontFamily,
                ObtenerDocumentoActual.SelectionFont.SizeInPoints, FontStyle.Regular);

            Font FuenteItalica = new Font(ObtenerDocumentoActual.SelectionFont.FontFamily,
                ObtenerDocumentoActual.SelectionFont.SizeInPoints, FontStyle.Italic);

            if (ObtenerDocumentoActual.SelectionFont.Italic)
            {
                ObtenerDocumentoActual.SelectionFont = FuenteRegular;
            }
            else
            {
                ObtenerDocumentoActual.SelectionFont = FuenteItalica;
            }
        }

        private void toolStripButtonSubrayado_Click(object sender, EventArgs e)
        {
            Font FuenteRegular = new Font(ObtenerDocumentoActual.SelectionFont.FontFamily,
                ObtenerDocumentoActual.SelectionFont.SizeInPoints, FontStyle.Regular);

            Font FuenteSubrayado = new Font(ObtenerDocumentoActual.SelectionFont.FontFamily,
                ObtenerDocumentoActual.SelectionFont.SizeInPoints, FontStyle.Underline);

            if (ObtenerDocumentoActual.SelectionFont.Underline)
            {
                ObtenerDocumentoActual.SelectionFont = FuenteRegular;
            }
            else
            {
                ObtenerDocumentoActual.SelectionFont = FuenteSubrayado;
            }
        }

        private void toolStripButtonTachado_Click(object sender, EventArgs e)
        {
            Font FuenteRegular = new Font(ObtenerDocumentoActual.SelectionFont.FontFamily,
                ObtenerDocumentoActual.SelectionFont.SizeInPoints, FontStyle.Regular);

            Font FuenteTachado = new Font(ObtenerDocumentoActual.SelectionFont.FontFamily,
                ObtenerDocumentoActual.SelectionFont.SizeInPoints, FontStyle.Strikeout);

            if (ObtenerDocumentoActual.SelectionFont.Strikeout)
            {
                ObtenerDocumentoActual.SelectionFont = FuenteRegular;
            }
            else
            {
                ObtenerDocumentoActual.SelectionFont = FuenteTachado;
            }
        }

        private void toolStripButtonMayusculas_Click(object sender, EventArgs e)
        {
            ObtenerDocumentoActual.SelectedText = ObtenerDocumentoActual.SelectedText.ToUpper();
        }

        private void toolStripButtonMinusculas_Click(object sender, EventArgs e)
        {
            ObtenerDocumentoActual.SelectedText = ObtenerDocumentoActual.SelectedText.ToLower();
        }

        private void toolStripButtonAgrandarFuente_Click(object sender, EventArgs e)
        {
            float NuevoTamanoFuente = ObtenerDocumentoActual.SelectionFont.SizeInPoints + 2;

            Font NuevoTamano = new Font(ObtenerDocumentoActual.SelectionFont.Name,
                NuevoTamanoFuente, ObtenerDocumentoActual.SelectionFont.Style);

            this.m_fontFuenteSeleccionada = NuevoTamano;
            ObtenerDocumentoActual.SelectionFont = NuevoTamano;
        }

        private void toolStripButtonReducirFuente_Click(object sender, EventArgs e)
        {
            float NuevoTamanoFuente = ObtenerDocumentoActual.SelectionFont.SizeInPoints - 2;

            Font NuevoTamano = new Font(ObtenerDocumentoActual.SelectionFont.Name,
                NuevoTamanoFuente, ObtenerDocumentoActual.SelectionFont.Style);

            this.m_fontFuenteSeleccionada = NuevoTamano;
            ObtenerDocumentoActual.SelectionFont = NuevoTamano;
        }

        private void toolStripButtonColorTexto_Click(object sender, EventArgs e)
        {
            if (colorDialogColorFuente.ShowDialog() == DialogResult.OK)
            {
                ObtenerDocumentoActual.SelectionColor = colorDialogColorFuente.Color;
            }
        }

        private void toolStripComboBoxFamiliaFuentes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Font NuevaFuente = new Font(toolStripComboBoxFamiliaFuentes.SelectedItem.ToString(), 
                ObtenerDocumentoActual.SelectionFont.Size, 
                ObtenerDocumentoActual.SelectionFont.Style);

            this.m_fontFuenteSeleccionada = NuevaFuente;
            ObtenerDocumentoActual.SelectionFont = NuevaFuente;
        }

        private void toolStripComboBoxTamanoFuente_SelectedIndexChanged(object sender, EventArgs e)
        {
            float NuevoTamano;

            float.TryParse(toolStripComboBoxTamanoFuente.SelectedItem.ToString(), out NuevoTamano);

            Font NuevaFuente = new Font(ObtenerDocumentoActual.SelectionFont.Name, NuevoTamano,
                ObtenerDocumentoActual.SelectionFont.Style);

            this.m_fontFuenteSeleccionada = NuevaFuente;
            ObtenerDocumentoActual.SelectionFont = NuevaFuente;
        }


        #endregion ToolStripPanelSuperior Eventos

        #region ToolStripPanelIzquierdo Eventos

        private void nuevoToolStripButton_Click(object sender, EventArgs e)
        {
            AgregarPestana();
        }

        private void cerrarToolStripButton_Click(object sender, EventArgs e)
        {
            EliminarPestana();
        }

        private void abrirToolStripButton_Click(object sender, EventArgs e)
        {
            Abrir();
        }

        private void guardarToolStripButton_Click(object sender, EventArgs e)
        {
            Guardar();
        }

        private void cortarToolStripButton_Click(object sender, EventArgs e)
        {
            Cortar();
        }

        private void copiarToolStripButton_Click(object sender, EventArgs e)
        {
            Copiar();
        }

        private void pegarToolStripButton_Click(object sender, EventArgs e)
        {
            Pegar();
        }

        private void ayudaToolStripButton_Click(object sender, EventArgs e)
        {
            // Ayuda()...
        }


        #endregion ToolStripPanelIzquierdo Eventos

        #region ContextStripMenuDocumento Eventos

        private void deshacerToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            Deshacer();
        }

        private void rehacerToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            Rehacer();
        }

        private void cortarToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            Cortar();
        }

        private void copiarToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            Copiar();
        }

        private void pegarToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            Pegar();
        }

        private void seleccionarTodoToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            SeleccionarTodo();
        }

        #endregion ContextStripMenuDocumento Eventos

        #region ContextStripMenuPestana Eventos

        private void cerrarToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            EliminarPestana();
        }

        private void cerrarOtrosToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            EliminarTodasLasPestanasMenosEsta();
        }

        private void cerrarALaDerechaToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            // EliminarPestanasPosteriores();
        }

        private void cerrarGuardadosToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            // EliminarPestanasGuardadas();
        }

        private void cerrarTodoToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            EliminarTodasLasPestanas();
        }

        #endregion ContextStripMenuPestana Eventos

        #region TimerPrincipal Eventos

        private void timerPrincipal_Tick(object sender, EventArgs e)
        {
            if (ObtenerDocumentoActual != null)
            {
                toolStripStatusLabelCantidadCaracteresDocumentoActual.Text = ObtenerDocumentoActual.Text.Length.ToString();
            }
            else
            {
                toolStripStatusLabelCantidadCaracteresDocumentoActual.Text = "";
            }
        }

        #endregion TimerPrincipal Eventos

    }
}
