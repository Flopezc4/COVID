﻿using COVID.DB;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using LiveCharts.Configurations;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COVID.MLP;
using SpreadsheetLight;

namespace COVID.Vista
{
    public partial class mlp : Form
    {
        database db;
        Mlp red;
        public mlp()
        {
            
            
            this.db = new database();
            this.red = entrenamiento.carga();
            InitializeComponent();
            Refresh();
            f5();
        }

        #region F5
        private void mlp_Load(object sender, EventArgs e)
        {

            //Refresh();
            //f5();

        }

        public void Refresh()
        {
            SQLiteCommand cmd = db.tabla1();
            DataTable dt = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
        }
        public void f5() 
        {
            cartesianChart1.Series.Clear();
            SeriesCollection series = new SeriesCollection();
            Dictionary<string, double> data = db.grafico1();
            List<double> valores = new List<double>();
            List<string> fechas = new List<string>();

            foreach (var i in data)
            {
                fechas.Add(i.Key);
                valores.Add(i.Value);
            }

            series.Add(new LineSeries() { Title = "Contagios", Values = new ChartValues<double>(valores) });
            cartesianChart1.Series = series;
            cartesianChart1.LegendLocation = LegendLocation.Right;
        }
        public void f5(double[] salida)
        {
            cartesianChart1.Series.Clear();
            SeriesCollection series = new SeriesCollection();
            Dictionary<string, double> data = db.grafico1();
            List<double> valores = new List<double>();
            List<string> fechas = new List<string>();
            List<double> prediccion = new List<double>();

            foreach (var i in data)
            {
                fechas.Add(i.Key);
                valores.Add(i.Value);
                prediccion.Add(i.Value);
            }
            foreach (var i in salida)
            {
                prediccion.Add(i);
            }

            series.Add(new LineSeries() { Title = "Contagios", Values = new ChartValues<double>(valores) });
            series.Add(new LineSeries() { Title = "Predicción", Values = new ChartValues<double>(prediccion)});
            cartesianChart1.Series = series;
            cartesianChart1.LegendLocation = LegendLocation.Right;
        }
        #endregion

        #region Botones

        private void button1_Click(object sender, EventArgs e)            ///btnEntrenar
        {
            entrenamiento.fit();
        }
        private void button2_Click(object sender, EventArgs e)            ///btnPredecir
        {
            List<double[]> entrada = db.datax();              
            double[] input = entrada[entrada.Count-1];
            double[] salida =db.NormInverse(red.Forward_propagation(input));

            // grid view 2 - Actualizacion
            int rowEscribir = dataGridView2.Rows.Count - 1;
            //dataGridView2.Rows.Add();
            dataGridView2.Rows[rowEscribir].Cells[0].Value = salida[0];
            dataGridView2.Rows[rowEscribir].Cells[1].Value = salida[1];
            dataGridView2.Rows[rowEscribir].Cells[2].Value = salida[2];
            dataGridView2.Rows[rowEscribir].Cells[3].Value = salida[3];
            dataGridView2.Rows[rowEscribir].Cells[4].Value = salida[4];
            dataGridView2.Rows[rowEscribir].Cells[5].Value = salida[5];
            dataGridView2.Rows[rowEscribir].Cells[6].Value = salida[6];
            dataGridView2.Rows[rowEscribir].Cells[7].Value = salida[7];
            dataGridView2.Rows[rowEscribir].Cells[8].Value = salida[8];
            dataGridView2.Rows[rowEscribir].Cells[9].Value = salida[9];

            f5(salida);
        }
        private void button3_Click(object sender, EventArgs e)            
        {
            this.SetVisibleCore(false);
            new principal().ShowDialog();
            this.Dispose();
        }
        private void button4_Click(object sender, EventArgs e)            ///btnGenerarReporte
        {
            SLDocument sl = new SLDocument();           ///objeto paquete
            SLStyle style = new SLStyle();              ///estilos
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();///guardado de archivo

            style.Font.FontSize = 15;
            style.Font.Bold = true;

            int numcol = 1;
            foreach (DataGridViewColumn column in dataGridView2.Columns)          ///recorrer columnas
            {
                sl.SetCellValue(1, numcol, column.HeaderText.ToString());
                sl.SetCellStyle(1, numcol, style);
                numcol++;
            }

            int numfila = 2; ///fila excel
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                sl.SetCellValue(numfila, 1, row.Cells[0].Value.ToString());      ///celda,columna,valor
                sl.SetCellValue(numfila, 2, row.Cells[1].Value.ToString());
                sl.SetCellValue(numfila, 3, row.Cells[2].Value.ToString());
                sl.SetCellValue(numfila, 4, row.Cells[3].Value.ToString());
                sl.SetCellValue(numfila, 5, row.Cells[4].Value.ToString());
                sl.SetCellValue(numfila, 6, row.Cells[5].Value.ToString());
                sl.SetCellValue(numfila, 7, row.Cells[6].Value.ToString());
                sl.SetCellValue(numfila, 8, row.Cells[7].Value.ToString());
                sl.SetCellValue(numfila, 9, row.Cells[8].Value.ToString());
                sl.SetCellValue(numfila, 10, row.Cells[9].Value.ToString());
                numfila++;
            }
            ///sl.SaveAs(@"C:\SW\ReporteMLP.xlsx");                     ///guardado por defecto
            saveFileDialog1.Title = "Guardar archivo";                  ///guardado por directorio
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "xlsx";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    sl.SaveAs(saveFileDialog1.FileName);
                    MessageBox.Show("Archivo exportado con éxito");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #endregion

        #region Tablas
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        #endregion
        
    }

}
