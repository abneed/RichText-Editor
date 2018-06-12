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
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace RichText_Editor
{
    public partial class FormMain : Form
    {
        #region Constantes

        /// <summary>
        ///   Establece la familia fuente predeterminada con que iniciara el editor.
        /// </summary>
        private const string FamiliaFuentePredeterminada = "Calibri";

        /// <summary>
        ///   Establece el tamaño de la fuente predeterminada con que iniciara el editor.
        /// </summary>
        private const int TamanoFuentePredeterminada = 15;

        #endregion Constantes

        #region Miembros Privados

        /// <summary>
        ///   Control donde cuenta la cantidad de pestañas activas en el editor.
        /// </summary>
        private int m_intConteoPestanas = 0;

        /// <summary>
        ///   Control donde almacena la familia fuente utilizada para su uso posterior.
        /// </summary>
        private Font m_fontFamiliaFuenteSeleccionada;

        #endregion

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.m_fontFamiliaFuenteSeleccionada = ObtenerFuentePredeterminado();
            tabControlPrincipal.ContextMenuStrip = contextMenuStripPestana;

            AgregarPestana();
            ObtenerColeccionFuente();
            TamanosFuente();
        }

        #region Propiedades

        /// <summary>
        ///   Propiedad donde se consulta el control RichTextBox que esta dentro de la pestaña
        ///   seleccionada actualmente por el usuario.
        /// </summary>
        public RichTextBox ObtenerDocumentoActual
        {
            get { return (RichTextBox)tabControlPrincipal.SelectedTab.Controls["Cuerpo"]; }
        }

        #endregion Propiedades

        #region Metodos

        #region Pestañas

        /// <summary>
        ///   Metodo que instancia un nuevo documento sin contenido dentro de una nueva pestaña.
        /// </summary>
        private void AgregarPestana()
        {
            // Se instancia un nuevo RichTextBox con las siguientes valores establecidas
            // dentro de sus propiedades.
            RichTextBox Cuerpo = new RichTextBox
            {
                Name = "Cuerpo",
                AcceptsTab = true,
                Dock = DockStyle.Fill,
                ContextMenuStrip = contextMenuStripDocumento,
                Font = this.m_fontFamiliaFuenteSeleccionada
            };

            // Se incrementa el conteo de pestañas...
            this.m_intConteoPestanas++;

            // Se genera un nombre para el nuevo documento.
            string TextoDocumento = "Sin título-" + this.m_intConteoPestanas;

            // Se crea la nueva pestaña (TabPage) con el nombre del nuevo documento como su titulo
            // y nombre del nuevo control.
            TabPage NuevaPagina = new TabPage
            {
                Name = TextoDocumento,
                Text = TextoDocumento
            };

            // Se agrega el nuevo RichTextBox dentro de la nueva pestaña (TabPage).
            NuevaPagina.Controls.Add(Cuerpo);

            // Se agrega la nueva pestaña (TabPage) dentro del control de pestañas (TabControl).
            tabControlPrincipal.TabPages.Add(NuevaPagina);
        }

        /// <summary>
        ///   Metodo que elimina la pestaña actual seleccionada.
        /// </summary>
        private void EliminarPestana()
        {
            if (tabControlPrincipal.TabPages.Count != 1)
            {
                tabControlPrincipal.TabPages.Remove(tabControlPrincipal.SelectedTab);
            }
            else
            {
                tabControlPrincipal.TabPages.Remove(tabControlPrincipal.SelectedTab);
                this.m_intConteoPestanas = 0;
                AgregarPestana();
            }
        }

        /// <summary>
        ///   Metodo que elimina todas las pestañas activas dentro del editor.
        /// </summary>
        private void EliminarTodasLasPestanas()
        {
            foreach (TabPage Page in tabControlPrincipal.TabPages)
            {
                tabControlPrincipal.TabPages.Remove(Page);
            }
            this.m_intConteoPestanas = 0;
            AgregarPestana();
        }

        /// <summary>
        ///   Método que elimina todas las pestañas activas, ha excepción de la pestaña
        ///   actual seleccionada por el usuario.
        /// </summary>
        private void EliminarTodasLasPestanasMenosEsta()
        {
            foreach (TabPage Page in tabControlPrincipal.TabPages)
            {
                if (Page.Name != tabControlPrincipal.SelectedTab.Name)
                {
                    tabControlPrincipal.TabPages.Remove(Page);
                }
            }
        }

        /// <summary>
        ///   Método que elimina todas las pestañas posteriores a la pestaña seleccionada
        ///   por el usuario.
        /// </summary>
        private void EliminarPestanasPosteriores()
        {
            for (int i = tabControlPrincipal.TabCount - 1; i > tabControlPrincipal.SelectedIndex; i--)
            {
                tabControlPrincipal.TabPages.RemoveAt(i);
            }
        }

        /// <summary>
        ///   Método que elimina todas las pestañas donde su contenido ya han sido guardadas por el usuario.
        /// </summary>
        private void EliminarPestanasGuardadas()
        {

        }

        #endregion Pestañas

        #region Guardar&Abrir

        /// <summary>
        ///   Método que abre un archivo existente.
        /// </summary>
        private void Abrir()
        {
            openFileDialogDocumento.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialogDocumento.Filter = "Formato de texto enriquecido (RTF)|*.rtf";

            if (openFileDialogDocumento.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Si se selecciono un nombre de archivo valido...
                if (openFileDialogDocumento.FileName.Length > 0)
                {
                    try
                    {
                        // Se genera una nueva pestaña.
                        AgregarPestana();

                        // Se busca y se selecciona la nueva pestaña generada.
                        tabControlPrincipal.SelectedTab = tabControlPrincipal.TabPages["Sin título-" + this.m_intConteoPestanas];

                        // Carga el contenido del archivo en el RichTextBox de la nueva pestaña.
                        ObtenerDocumentoActual.LoadFile(openFileDialogDocumento.FileName, RichTextBoxStreamType.RichText);
                        
                        // Se establece el nombre del archivo en el titulo de la pestaña y el nombre de la misma.
                        string NombreArchivo = Path.GetFileName(openFileDialogDocumento.FileName);
                        tabControlPrincipal.SelectedTab.Text = NombreArchivo;
                        tabControlPrincipal.SelectedTab.Name = NombreArchivo;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }

        /// <summary>
        ///   Método que guarda el documento activo.
        /// </summary>
        private void Guardar()
        {

            saveFileDialogDocumento.FileName = tabControlPrincipal.SelectedTab.Name;
            saveFileDialogDocumento.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialogDocumento.Filter = "Formato de texto enriquecido (RTF)|*.rtf";
            saveFileDialogDocumento.Title = "Guardar";

            if (saveFileDialogDocumento.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Si se selecciono un nombre de archivo valido...
                if (saveFileDialogDocumento.FileName.Length > 0)
                {
                    try
                    {
                        // Guarda el contenido del RichTextBox en la ruta del archivo establecida.
                        ObtenerDocumentoActual.SaveFile(saveFileDialogDocumento.FileName, RichTextBoxStreamType.RichText);

                        // Se establece el nombre del archivo en el titulo de la pestaña y el nombre de la misma.
                        string NombreArchivo = Path.GetFileName(saveFileDialogDocumento.FileName);
                        tabControlPrincipal.SelectedTab.Text = NombreArchivo;
                        tabControlPrincipal.SelectedTab.Name = NombreArchivo;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }

        /// <summary>
        ///   Método que guarda el documento con un nuevo nombre o formato.
        /// </summary>
        private void GuardarComo()
        {
            saveFileDialogDocumento.FileName = tabControlPrincipal.SelectedTab.Name;
            saveFileDialogDocumento.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialogDocumento.Filter = "Formato de texto enriquecido (RTF)|*.rtf";
            saveFileDialogDocumento.Title = "Guardar como";

            if (saveFileDialogDocumento.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Si se selecciono un nombre de archivo valido...
                if (saveFileDialogDocumento.FileName.Length > 0)
                {
                    try
                    {
                        // Guarda el contenido del RichTextBox en la ruta del archivo establecida.
                        ObtenerDocumentoActual.SaveFile(saveFileDialogDocumento.FileName, RichTextBoxStreamType.RichText);

                        // Se establece el nombre del archivo en el titulo de la pestaña y el nombre de la misma.
                        string NombreArchivo = Path.GetFileName(saveFileDialogDocumento.FileName);
                        tabControlPrincipal.SelectedTab.Text = NombreArchivo;
                        tabControlPrincipal.SelectedTab.Name = NombreArchivo;
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

        /// <summary>
        ///   Método que deshace la última acción hecha por el usuario.
        /// </summary>
        private void Deshacer()
        {
            ObtenerDocumentoActual.Undo();
        }

        /// <summary>
        ///   Método que rehace la última acción hecha por el usuario.
        /// </summary>
        private void Rehacer()
        {
            ObtenerDocumentoActual.Redo();
        }

        /// <summary>
        ///   Método que corta la selección del lienzo y la coloca en el Portapapeles.
        /// </summary>
        private void Cortar()
        {
            ObtenerDocumentoActual.Cut();
        }

        /// <summary>
        ///   Método que copia la selección del lienzo y la coloca en el Portapapeles.
        /// </summary>
        private void Copiar()
        {
            ObtenerDocumentoActual.Copy();
        }

        /// <summary>
        ///   Método que pega el contenido del Portapapeles.
        /// </summary>
        private void Pegar()
        {
            ObtenerDocumentoActual.Paste();
        }

        /// <summary>
        ///   Método que selecciona todo el texto del documento actual.
        /// </summary>
        private void SeleccionarTodo()
        {
            ObtenerDocumentoActual.SelectAll();
        }

        #endregion Texto

        #region General

        /// <summary>
        ///   Método para obtener todas las familias fuente instaladas en el sistema,
        ///   y establecerlas en el control toolStripComboBoxFamiliaFuentes.
        /// </summary>
        private void ObtenerColeccionFuente()
        {
            InstalledFontCollection FuenteIns = new InstalledFontCollection();

            foreach (FontFamily item in FuenteIns.Families)
            {
                toolStripComboBoxFamiliaFuentes.Items.Add(item.Name);
            }

            toolStripComboBoxFamiliaFuentes.SelectedIndex = toolStripComboBoxFamiliaFuentes.FindString(FamiliaFuentePredeterminada);
        }

        /// <summary>
        ///   Método para establecer los valores que representaran los tamaños de la fuente
        ///   dentro del control toolStripComboBoxTamanoFuente.
        /// </summary>
        private void TamanosFuente()
        {
            for (int i = 0; i <= 75; i++)
            {
                toolStripComboBoxTamanoFuente.Items.Add(i);
            }

            toolStripComboBoxTamanoFuente.SelectedIndex = TamanoFuentePredeterminada;
        }

        /// <summary>
        ///   Método para obtener la familia fuente predeterminada.
        /// </summary>
        /// <returns>Devuelve un <see cref="Font"/> que representa la familia fuente predeterminada.</returns>
        private Font ObtenerFuentePredeterminado()
        {
            return new Font(FamiliaFuentePredeterminada, TamanoFuentePredeterminada, FontStyle.Regular);
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

        /// <summary>
        ///   Evento que se activa cuando el usuario haga clic sobre el control toolStripButtonNegrita;
        ///   cambia una fuente en negrita.
        /// </summary>
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

        /// <summary>
        ///   Evento que se activa cuando el usuario haga clic sobre el control toolStripButtonCursiva;
        ///   cambia una fuente en cursiva.
        /// </summary>
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

        /// <summary>
        ///   Evento que se activa cuando el usuario haga clic sobre el control toolStripButtonSubrayado;
        ///   dibuja una línea debajo del texto.
        /// </summary>
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

        /// <summary>
        ///   Evento que se activa cuando el usuario haga clic sobre el control toolStripButtonTachado;
        ///   dibuja una línea en medio del texto.
        /// </summary>
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

        /// <summary>
        ///   Evento que se activa cuando el usuario haga clic sobre el control toolStripButtonMayusculas;
        ///   convierte el texto en mayúsculas.
        /// </summary>
        private void toolStripButtonMayusculas_Click(object sender, EventArgs e)
        {
            ObtenerDocumentoActual.SelectedText = ObtenerDocumentoActual.SelectedText.ToUpper();
        }

        /// <summary>
        ///   Evento que se activa cuando el usuario haga clic sobre el control toolStripButtonMinusculas;
        ///   convierte el texto en minúsculas.
        /// </summary>
        private void toolStripButtonMinusculas_Click(object sender, EventArgs e)
        {
            ObtenerDocumentoActual.SelectedText = ObtenerDocumentoActual.SelectedText.ToLower();
        }

        /// <summary>
        ///   Evento que se activa cuando el usuario haga clic sobre el control toolStripButtonAgrandarFuente;
        ///   aumenta el tamaño de la fuente.
        /// </summary>
        private void toolStripButtonAgrandarFuente_Click(object sender, EventArgs e)
        {
            float NuevoTamanoFuente = ObtenerDocumentoActual.SelectionFont.SizeInPoints + 2;

            Font NuevoTamano = new Font(ObtenerDocumentoActual.SelectionFont.Name,
                NuevoTamanoFuente, ObtenerDocumentoActual.SelectionFont.Style);

            this.m_fontFamiliaFuenteSeleccionada = NuevoTamano;
            ObtenerDocumentoActual.SelectionFont = NuevoTamano;
        }

        /// <summary>
        ///   Evento que se activa cuando el usuario haga clic sobre el control toolStripButtonReducirFuente;
        ///   reduce el tamaño de la fuente.
        /// </summary>
        private void toolStripButtonReducirFuente_Click(object sender, EventArgs e)
        {
            float NuevoTamanoFuente = ObtenerDocumentoActual.SelectionFont.SizeInPoints - 2;

            Font NuevoTamano = new Font(ObtenerDocumentoActual.SelectionFont.Name,
                NuevoTamanoFuente, ObtenerDocumentoActual.SelectionFont.Style);

            this.m_fontFamiliaFuenteSeleccionada = NuevoTamano;
            ObtenerDocumentoActual.SelectionFont = NuevoTamano;
        }

        /// <summary>
        ///   Evento que se activa cuando el usuario haga clic sobre el control toolStripButtonColorTexto;
        ///   cambia el color del texto.
        /// </summary>
        private void toolStripButtonColorTexto_Click(object sender, EventArgs e)
        {
            if (colorDialogColorFuente.ShowDialog() == DialogResult.OK)
            {
                ObtenerDocumentoActual.SelectionColor = colorDialogColorFuente.Color;
            }
        }

        /// <summary>
        ///   Evento que se activa cuando el indice seleccionado cambio del control toolStripComboBoxFamiliaFuentes;
        ///   cambia la familia de fuentes.
        /// </summary>
        private void toolStripComboBoxFamiliaFuentes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Font NuevaFuente = new Font(toolStripComboBoxFamiliaFuentes.SelectedItem.ToString(), 
                ObtenerDocumentoActual.SelectionFont.Size, 
                ObtenerDocumentoActual.SelectionFont.Style);

            this.m_fontFamiliaFuenteSeleccionada = NuevaFuente;
            ObtenerDocumentoActual.SelectionFont = NuevaFuente;
        }

        /// <summary>
        ///   Evento que se activa cuando el indice seleccionado cambio del control toolStripComboBoxTamanoFuente;
        ///   cambia el tamaño de la fuente.
        /// </summary>
        private void toolStripComboBoxTamanoFuente_SelectedIndexChanged(object sender, EventArgs e)
        {
            float NuevoTamano;

            float.TryParse(toolStripComboBoxTamanoFuente.SelectedItem.ToString(), out NuevoTamano);

            Font NuevaFuente = new Font(ObtenerDocumentoActual.SelectionFont.Name, NuevoTamano,
                ObtenerDocumentoActual.SelectionFont.Style);

            this.m_fontFamiliaFuenteSeleccionada = NuevaFuente;
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
            EliminarPestanasPosteriores();
        }

        private void cerrarGuardadosToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            EliminarPestanasGuardadas();
        }

        private void cerrarTodoToolStripMenuItemContext_Click(object sender, EventArgs e)
        {
            EliminarTodasLasPestanas();
        }

        #endregion ContextStripMenuPestana Eventos

        #region TimerPrincipal Eventos

        /// <summary>
        ///   <see cref="Timer"/> que se encarga de verificar la cantidad de caracteres que contiene
        ///   dentro del documento actual.
        /// </summary>
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
