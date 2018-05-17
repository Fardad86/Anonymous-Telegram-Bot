﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using HarfeToBeBot_v2._0.Utilities;
using HarfeToBeBot_v2._0.Model;

namespace HarfeToBeBot_v2._0.Controller {
    class DatabaseHandler {
        SqlConnection SqlConnection;
        public DatabaseHandler() {
            // open connection
            try {
                SqlConnection = new SqlConnection(connectionString: "Data Source=.;Initial Catalog=BOT_DB;Integrated Security=True");
                SqlConnection.Open();
            } catch(Exception ex) {
                ErrorHandler.SetError(source: "DatabaseHandler()", error: ex.Message);
                throw new Exception(message: "DatabaseHandler", innerException: ex);
            }
        }

        public bool RegisterUser(Model.NewUser newUser) {
            try {
                SqlCommand cmd = new SqlCommand(cmdText: "INSERT INTO tbl_users (Id,FullName,UserName,ContactCode,ProfilePhoto) VALUES (@id,@name,@user,@contact,@image)", connection: SqlConnection);
                cmd.Parameters.AddWithValue(parameterName: "@id", value: newUser.Id);
                cmd.Parameters.AddWithValue(parameterName: "@name", value: newUser.FullName);
                cmd.Parameters.AddWithValue(parameterName: "@user", value: newUser.UserName);
                cmd.Parameters.AddWithValue(parameterName: "@contact", value: newUser.ContactCode);
                cmd.Parameters.AddWithValue(parameterName: "@image", value: Utilities.Helper.GetBytesFromImage(image: newUser.ProfilePicture));
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand(cmdText: "INSERT INTO tbl_requests (Id) VALUES (@id)", connection: SqlConnection);
                cmd.Parameters.AddWithValue("@id", value: newUser.Id);
                return true;
            } catch (Exception ex) {
                ErrorHandler.SetError(source: "RegisterUser", error: ex.Message);
                return false;
            }
        }

        public bool UserExists(long id) {
            try {
                SqlCommand command = new SqlCommand(cmdText: "SELECT COUNT(*) FROM tbl_users WHERE (Id=@id)", connection: SqlConnection);
                command.Parameters.AddWithValue(parameterName: "@id", value: id);
                int userExists = (int)command.ExecuteScalar();
                if (userExists > 0)
                    return true;
                return false;
            } catch (Exception ex) {
                ErrorHandler.SetError(source: "UserExists(int)", error: ex.Message);
                return false;
            }
        }

        public bool UserExists(string contactCode) {
            try {
                SqlCommand command = new SqlCommand(cmdText: "SELECT COUNT(*) FROM tbl_users WHERE (ContactCode=@contact)", connection: SqlConnection);
                command.Parameters.AddWithValue(parameterName: "@contact", value: contactCode);
                int userExists = (int)command.ExecuteScalar();
                if (userExists > 0)
                    return true;
                return false;
            } catch (Exception ex) {
                ErrorHandler.SetError(source: "UserExists(string)", error: ex.Message);
                return false;
            }
        }

        public bool IsContactNameSet(long id) {
            try {
                SqlCommand command = new SqlCommand(cmdText: "SELECT ContactName FROM tbl_users WHERE (Id=@id)", connection: SqlConnection);
                command.Parameters.AddWithValue(parameterName: "@id", value: id);
                var contactName = command.ExecuteScalar();

                if (contactName != null)
                    return true;
                return false;
            } catch(Exception ex) {
                ErrorHandler.SetError(source: "IsContactNameSet", error: ex.Message);
                return false;
            }
        }

        public bool SetContactName(long id, string name) {
            try {
                SqlCommand command = new SqlCommand(cmdText: "UPDATE tbl_users SET ContactName=@cName WHERE Id=@id", connection: SqlConnection);
                command.Parameters.AddWithValue(parameterName: "@cName", value: name);
                command.Parameters.AddWithValue(parameterName: "@id", value: id);
                command.ExecuteNonQuery();
                return true;
            } catch(Exception ex) {
                ErrorHandler.SetError(source: "SetContactName", error: ex.Message);
                return false;
            }
        }

        public string GetFullNameByContactCode(string contactCode) {
            try {
                SqlCommand command = new SqlCommand(cmdText: "SELECT FullName FROM tbl_users WHERE (ContactCode=@contact)", connection: SqlConnection);
                command.Parameters.AddWithValue("@contact", contactCode);
                var fullName = (string)command.ExecuteScalar();
                if (fullName != null) {
                    return fullName.ToString();
                }

                return null;
            } catch (Exception ex) {
                ErrorHandler.SetError(source: "GetFullNameFromContactCode", error: ex.Message);
                return null;
            }
        }

        public bool EditUserRequest(long id, string contactCode = "", UserRequests userRequests) {
            try {
                SqlCommand command = new SqlCommand(cmdText: "UPDATE tbl_requests SET (Request=@request,ContactCode=@contact) WHERE Id=@id", connection: SqlConnection);
                command.Parameters.AddWithValue(parameterName: "@requst", value: (int)userRequests);
                command.Parameters.AddWithValue(parameterName: "@contact", value: contactCode);
                command.Parameters.AddWithValue(parameterName: "@id", value: id);
                command.ExecuteNonQuery();

                return true;
            } catch (Exception ex) {
                ErrorHandler.SetError(source: "EditUserRequest", error: ex.Message);
                return false;
            }
        }
    }
}