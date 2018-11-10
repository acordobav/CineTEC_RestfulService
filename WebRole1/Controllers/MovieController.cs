﻿using System;
using Npgsql;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    public class MovieController : Controller
    {
        /// <summary>
        /// Metodo para obtener todas las peliculas
        /// </summary>
        /// <returns>
        /// Una lista en formato Json con todas los peliculas
        /// </returns>
        [HttpGet]
        public ActionResult GetAll()
        {
            //Se abre la conexion entre C# y la base de datos
            NpgsqlConnection connection = new NpgsqlConnection(Connection.connectionString);
            connection.Open();

            //Se declara el comando SQL a ejecutar
            string sqlQuery = "SELECT * FROM MOVIE";

            //Se ejecuta el comando
            NpgsqlDataAdapter sqlData = new NpgsqlDataAdapter(sqlQuery, connection);

            //Se almacena la respuesta
            DataSet dataSet = new DataSet();
            sqlData.Fill(dataSet);

            //Se cierra la conexion
            connection.Close();

            //Se crea una variable con los objetos recuperados
            var data = dataSet.Tables[0].AsEnumerable().Select(x => new Movie
            {
                Id_movie = x.Field<int>("Id_movie"),
                O_name = x.Field<string>("O_name"),
                M_name = x.Field<string>("M_name"),
                I_name = x.Field<string>("I_name"),
                Duration = x.Field<int>("Duration"),
                Id_rating = x.Field<int>("Id_rating")
            });

            //Se transforma la informacion obtenida a formato Json
            return Json(new { data = data.ToList() }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Metodo para obtener una pelicula en especifico
        /// </summary>
        /// <param name="id">Identificador de la pelicula</param>
        /// <returns>
        /// Json con la respuesta enviada por la base de datos
        /// </returns>
        [HttpGet]
        public ActionResult Get(int id)
        {
            try
            {
                //Se abre la conexion entre C# y la base de datos
                NpgsqlConnection connection = new NpgsqlConnection(Connection.connectionString);
                connection.Open();

                //Se declara la consulta SQL a ejecutar
                string sqlQuery = "SELECT * FROM MOVIE WHERE Id_movie = " + id.ToString();

                //Se ejecuta la consulta
                NpgsqlDataAdapter sqlData = new NpgsqlDataAdapter(sqlQuery, connection);

                //Se almacena la respuesta
                DataSet dataSet = new DataSet();
                sqlData.Fill(dataSet);

                //Se cierra la conexion
                connection.Close();

                //Se crea una variable con los objetos recuperados
                var data = dataSet.Tables[0].AsEnumerable().Select(x => new Movie
                {
                    Id_movie = x.Field<int>("Id_movie"),
                    O_name = x.Field<string>("O_name"),
                    M_name = x.Field<string>("M_name"),
                    I_name = x.Field<string>("I_name"),
                    Duration = x.Field<int>("Duration"),
                    Id_rating = x.Field<int>("Id_rating")
                });

                //Se transforma la informacion obtenida a formato Json
                return Json(new { data = data.ToList() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)//Si ocurrio algun error se retorna un Json indicandolo
            {
                return Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Metodo para almacenar una nueva pelicula
        /// </summary>
        /// <param name="movie">Objeto con la informacion a insertar</param>
        /// <returns>
        /// Json con un data = success si la operacion se realizo con exito, data = error en caso contrario
        /// </returns>
        [HttpPost]
        public ActionResult Create(Movie movie)
        {
            try
            {
                //Se abre la conexion entre C# y la base de datos
                NpgsqlConnection connection = new NpgsqlConnection(Connection.connectionString);
                connection.Open();

                //Se declara el comando SQL a ejecutar
                string sqlQuery = "INSERT INTO MOVIE (O_name, M_name, I_name, Duration, Id_rating) VALUES(@O_name, @M_name, @I_name, @Duration, @Id_rating)";

                //Se crea el objeto a cargo de realizar la consulta
                IDbCommand command = connection.CreateCommand();
                command.CommandText = sqlQuery;

                //Se declara el parametro correspondiente a O_name
                var paramOname = command.CreateParameter();
                paramOname.ParameterName = "O_name";
                paramOname.Value = movie.O_name;
                command.Parameters.Add(paramOname);

                //Se declara el parametro correspondiente a M_name
                var paramMname = command.CreateParameter();
                paramMname.ParameterName = "M_name";
                paramMname.Value = movie.M_name;
                command.Parameters.Add(paramMname);

                //Se declara el parametro correspondiente a I_name
                var paramIname = command.CreateParameter();
                paramIname.ParameterName = "I_name";
                paramIname.Value = movie.I_name;
                command.Parameters.Add(paramIname);

                //Se declara el parametro correspondiente a Duration
                var paramDuration = command.CreateParameter();
                paramDuration.ParameterName = "Duration";
                paramDuration.Value = movie.Duration;
                command.Parameters.Add(paramDuration);

                //Se declara el parametro correspondiente a Id_rating
                var paramIdRating = command.CreateParameter();
                paramIdRating.ParameterName = "Id_rating";
                paramIdRating.Value = movie.Id_rating;
                command.Parameters.Add(paramIdRating);

                //Se ejecuta el comando, se almacena el resultado en una variable
                int result = command.ExecuteNonQuery();

                //Se cierra la conexion
                connection.Close();

                //Si la operacion fue exitosa
                if (result > 0)
                {
                    return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                }
                //En caso de que ocurriera un error
                else
                {
                    return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Metodo para editar los datos de una pelicula
        /// </summary>
        /// <param name="movie">Objeto que almacena la informacion de la pelicula que se desea editar</param>
        /// <returns>
        /// Json con la respuesta obtenida a partir del exito de la operacion
        /// </returns>
        [HttpPut]
        public ActionResult Edit(Movie movie)
        {
            try
            {
                //Se abre la conexion entre C# y la base de datos
                NpgsqlConnection connection = new NpgsqlConnection(Connection.connectionString);
                connection.Open();

                //Se declara el comando SQL a ejecutar
                string sqlQuery = "UPDATE MOVIE SET O_name = @O_name, M_name = @M_name, I_name = @I_name, Duration = @Duration, Id_rating = @Id_rating WHERE Id_movie = @Id_movie";

                //Se crea el objeto a cargo de realizar la consulta
                IDbCommand command = connection.CreateCommand();
                command.CommandText = sqlQuery;

                //Se declara el parametro correspondiente a Id_movie
                var paramId = command.CreateParameter();
                paramId.ParameterName = "Id_movie";
                paramId.Value = movie.Id_movie;
                command.Parameters.Add(paramId);

                //Se declara el parametro correspondiente a O_name
                var paramOname = command.CreateParameter();
                paramOname.ParameterName = "O_name";
                paramOname.Value = movie.O_name;
                command.Parameters.Add(paramOname);

                //Se declara el parametro correspondiente a M_name
                var paramMname = command.CreateParameter();
                paramMname.ParameterName = "M_name";
                paramMname.Value = movie.M_name;
                command.Parameters.Add(paramMname);

                //Se declara el parametro correspondiente a I_name
                var paramIname = command.CreateParameter();
                paramIname.ParameterName = "I_name";
                paramIname.Value = movie.I_name;
                command.Parameters.Add(paramIname);

                //Se declara el parametro correspondiente a Duration
                var paramDuration = command.CreateParameter();
                paramDuration.ParameterName = "Duration";
                paramDuration.Value = movie.Duration;
                command.Parameters.Add(paramDuration);

                //Se declara el parametro correspondiente a Id_rating
                var paramIdRating = command.CreateParameter();
                paramIdRating.ParameterName = "Id_rating";
                paramIdRating.Value = movie.Id_rating;
                command.Parameters.Add(paramIdRating);

                //Se ejecuta el comando, se almacena el resultado en una variable
                int result = command.ExecuteNonQuery();

                //Se cierra la conexion
                connection.Close();

                //Si la operacion fue exitosa
                if (result > 0)
                {
                    return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                }
                //En caso de que ocurriera un error
                else
                {
                    return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Metodo para eliminar una pelicula
        /// </summary>
        /// <param name="id">Identificador de la pelicula</param>
        /// <returns>
        /// Json indicando el resultado de la operacion
        /// </returns>
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                //Se abre la conexion entre C# y la base de datos
                NpgsqlConnection connection = new NpgsqlConnection(Connection.connectionString);
                connection.Open();

                //Se declara el comando SQL a ejecutar
                string sqlQuery = "DELETE FROM MOVIE WHERE Id_movie = @Id_movie";

                //Se crea el objeto a cargo de realizar la consulta
                IDbCommand command = connection.CreateCommand();
                command.CommandText = sqlQuery;

                //Se declara el parametro correspondiente a Id_movie
                var paramId = command.CreateParameter();
                paramId.ParameterName = "Id_movie";
                paramId.Value = id;
                command.Parameters.Add(paramId);

                //Se ejecuta el comando, se almacena el resultado en una variable
                int result = command.ExecuteNonQuery();

                //Se cierra la conexion
                connection.Close();

                //Se retorna un Json indicando que la operacion fue exitosa
                return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e) //Caso en que ocurrio un error durante el proceso
            {
                return Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}