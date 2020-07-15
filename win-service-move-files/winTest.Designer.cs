namespace win_service_move_files
{
    partial class winTest
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.tmrWin = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.tmrWin)).BeginInit();
            // 
            // tmrWin
            // 
            this.tmrWin.Enabled = true;
            this.tmrWin.Interval = 30000D;
            this.tmrWin.Elapsed += new System.Timers.ElapsedEventHandler(this.tmrWin_Elapsed);
            // 
            // winTest
            // 
            this.ServiceName = "winTest";
            ((System.ComponentModel.ISupportInitialize)(this.tmrWin)).EndInit();

        }

        #endregion

        private System.Timers.Timer tmrWin;
    }
}
