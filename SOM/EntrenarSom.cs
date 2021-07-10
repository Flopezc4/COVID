﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using COVID.DB;

namespace COVID.SOM
{
    class EntrenarSom
    {
        static som self;
        static matriz mapa;
        static database db;
        static List<double[]> entrada;
        static string SomPath = @"D:\SW\EntrenamientoSOM.bin";

        public static void fit()
        {
            db = new database();
            entrada = db.datax();

            // datax
            mapa = new matriz(14, 10, 10);
            self = new som(mapa);

            self.entrenar(entrada,0.1,1000);  //REVISAR VALORES

            FileStream fs = new FileStream(SomPath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, self);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("No se pudo serializar, motivo: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }



        }
        public static void clasificar()
        {
            //self.clasificar();
        }
        public static som carga()
        {
            FileStream fs = new FileStream(SomPath, FileMode.Open); 
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                self = (som)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Error en deserializacion, motivo :" + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

            return self;
        }
    }
}
