using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NailsChekin.Models
{
    class XLDatabase
    {
        public const int Xem = 0;
        public const int Xoa = 1;
        public const int TaoMoi = 2;
        public const int CapNhat = 3;
        public const int Duyet = 4;

        //string chuoiKN = @"Server=45.119.82.18\SQLEXPRESS2014,1289; Database=DataCenter; User ID=vs; pwd=vietSo@12890";
        string chuoiKN = @"Server=95.217.32.253; Database=DataCenter; User ID=vs;pwd=vs@12890";

        public XLDatabase()
        {

        }

        public DataTable DocBang(string cauLenh)
        {
            using (SqlDataAdapter boDocGhi = new SqlDataAdapter(cauLenh, chuoiKN))
            {
                boDocGhi.SelectCommand.CommandTimeout = int.MaxValue;
                DataTable KQ = new DataTable();
                boDocGhi.Fill(KQ);
                return KQ;
            }

            //using (OleDbDataAdapter boDocGhi = new OleDbDataAdapter(cauLenh, chuoiKN))
            //{
            //    DataTable KQ = new DataTable();
            //    boDocGhi.Fill(KQ);
            //    return KQ;
            //}
        }

        public DataTable DocBang(string cauLenh, out SqlDataAdapter da)
        {
            da = new SqlDataAdapter(cauLenh, chuoiKN);
            da.SelectCommand.CommandTimeout = int.MaxValue;

            DataTable KQ = new DataTable();
            da.Fill(KQ);
            SqlCommandBuilder phatSinhLenh = new SqlCommandBuilder(da);
            return KQ;
        }

        public bool Thuc_Hien_Lenh(string lenh)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKN))
                {
                    ketNoi.Open();
                    SqlCommand cm = new SqlCommand(lenh, ketNoi);
                    if (cm.ExecuteNonQuery() > 0)
                    {
                        string msg = WriteToFile("Execute at {0}: " + lenh);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string Thuc_Hien_Lenh_STR(string lenh)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKN))
                {
                    ketNoi.Open();
                    SqlCommand cm = new SqlCommand(lenh, ketNoi);
                    if (cm.ExecuteNonQuery() > 0)
                        return "";

                    return "Error: " + lenh;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public object Thuc_Thi_Scalar(string lenh)
        {
            using (SqlConnection ketNoi = new SqlConnection(chuoiKN))
            {
                ketNoi.Open();
                SqlCommand cm = new SqlCommand(lenh, ketNoi);

                return cm.ExecuteScalar();
            }
        }


        #region SQL với param


        public DataTable GetTable(string command, params string[] values)
        {
            //SQL injection
            //command = KillCharsSelect(command);
            //command = KillCharsOnlySelect(command);
            //command = SafeSqlLiteral(command);
            //command = EscapeQuotes(command);

            ////Check Injecttion
            //if (Utilitys.checkForSQLInjection(command))
            //    return null;

            using (SqlDataAdapter boDocGhi = new SqlDataAdapter(command, chuoiKN))
            {
                boDocGhi.SelectCommand.CommandTimeout = int.MaxValue;

                boDocGhi.SelectCommand.Parameters.Clear();
                if (values != null)
                {
                    //Param name
                    var parameters = this.getParameters(command);

                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i].Length <= 8000)
                            boDocGhi.SelectCommand.Parameters.Add(parameters[i], SqlDbType.VarChar);
                        else
                            boDocGhi.SelectCommand.Parameters.Add(parameters[i], SqlDbType.Text);

                        boDocGhi.SelectCommand.Parameters[parameters[i]].Value = values[i];
                    }
                }

                DataTable KQ = new DataTable();
                boDocGhi.Fill(KQ);
                return KQ;
            }

        }

        public bool ExecuteNonQuery(string command, params string[] values)
        {
            //command = KillCharsSelect(command);
            //command = SafeSqlLiteral(command);
            //command = EscapeQuotes(command);

            ////Check Injecttion
            //if (Utilitys.checkForSQLInjection(command))
            //    return false;

            try
            {
                using (SqlConnection connection = new SqlConnection(chuoiKN))
                {
                    connection.Open();
                    SqlCommand cm = new SqlCommand(command, connection);

                    cm.Parameters.Clear();
                    if (values != null)
                    {
                        //Param name
                        var parameters = this.getParameters(command);

                        for (int i = 0; i < values.Length; i++)
                        {
                            cm.Parameters.Add(parameters[i], SqlDbType.VarChar);
                            cm.Parameters[parameters[i]].Value = values[i];
                        }
                    }

                    cm.CommandTimeout = int.MaxValue;
                    if (cm.ExecuteNonQuery() > 0)
                        return true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public object ExecuteScalar(string command, params string[] values)
        {
            //SQL injection
            //command = KillCharsSelect(command);
            //command = SafeSqlLiteral(command);
            //command = EscapeQuotes(command);

            ////Check Injecttion
            //if (Utilitys.checkForSQLInjection(command))
            //    return null;

            using (SqlConnection connection = new SqlConnection(chuoiKN))
            {
                connection.Open();
                SqlCommand cm = new SqlCommand(command, connection);

                cm.Parameters.Clear();
                if (values != null)
                {
                    //Param name
                    var parameters = this.getParameters(command);

                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i].Length <= 8000)
                            cm.Parameters.Add(parameters[i], SqlDbType.VarChar);
                        else
                            cm.Parameters.Add(parameters[i], SqlDbType.Text);

                        cm.Parameters[parameters[i]].Value = values[i];
                    }
                }


                cm.CommandTimeout = int.MaxValue;
                return cm.ExecuteScalar();
            }
        }

        public string[] getParameters(string command)
        {
            var parameters = command.Split(' ')
                .Where(x => x.Contains("@"))
                .Select(x => x
                    .Replace("=", "")
                    .Replace("%", "")
                    .Replace("'", "")
                    .Replace("\"", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace(",", "")
            ).ToArray();

            parameters = parameters.Select(x => "@" + x.Split('@').Last()).ToArray();

            return parameters;
        }


        #endregion



        public SqlDataReader getData(SqlCommand comand)
        {
            SqlDataReader reader = null;

            return reader;
        }

        public DataTable getTableFromProc(string procName, string[] paraName, string[] paraValue)
        {
            DataTable KQ = new DataTable();

            using (SqlConnection ketNoi = new SqlConnection(chuoiKN))
            {
                ketNoi.Open();
                SqlCommand command = new SqlCommand(procName, ketNoi);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = int.MaxValue;

                if (paraName.Length > 0)
                {
                    for (int i = 0; i < paraName.Length; i++)
                    {
                        command.Parameters.Add(paraName[i], SqlDbType.NVarChar);
                        command.Parameters[paraName[i]].Value = paraValue[i];
                        command.Parameters[paraName[i]].Direction = ParameterDirection.Input;
                    }
                }

                SqlDataReader dr = command.ExecuteReader();
                if (dr != null)
                {
                    KQ.Load(dr);
                }

                ketNoi.Close();
                command.Dispose();
            }

            return KQ;
        }

        public string[] getQuyen(string userId, string maungdung)
        {
            //Theo thu tu: XEM - XOA - TAO MOI - CHINH SUA - DUYET
            string[] quyen = new string[] { "0", "0", "0", "0", "0" };

            try
            {
                string query = "select max(xem) as xem, max(xoa) as xoa, max(taomoi) as taomoi, max(sua) as sua, max(chot) as chot " +
                               " from NhomQuyen_ChucNang_ChiTiet  " +
                               " where nhomquyen_fk in ( select nhomquyen_fk from NhanVien_Quyen_NhomQuyen where nhanvien_fk = '" + userId + "' ) and chucnang_fk = '" + maungdung + "'";

                //DataTable dt = this.DocBang("select xem, xoa, taomoi, sua from NhanVien_PhanQuyen_UngDung where nhanvien_fk = '" + userId + "' and ungdung_fk = '" + maungdung + "'");

                DataTable dt = this.DocBang(query);
                if (dt.Rows.Count > 0)
                {
                    quyen[0] = dt.Rows[0]["xem"].ToString();
                    quyen[1] = dt.Rows[0]["xoa"].ToString();
                    quyen[2] = dt.Rows[0]["taomoi"].ToString();
                    quyen[3] = dt.Rows[0]["sua"].ToString();
                    quyen[4] = dt.Rows[0]["chot"].ToString();
                }
            }
            catch { }

            return quyen;
        }

        public string ChuanHoa(string input)
        {
            string kq = Regex.Replace(input, "'", "''");
            return kq;
        }


        public string Un_FormatNumber(string input)
        {
            string kq = Regex.Replace(input, ",", "");
            return kq;
        }


        public string ForMatNumber(string input)
        {
            try
            {
                if (input.Trim().Length <= 0)
                    return "0";

                string kq = double.Parse(input).ToString("#,#", CultureInfo.InvariantCulture);
                if (kq.Trim().Length <= 0)
                    kq = "0";

                return kq;
            }
            catch (Exception e)
            {
                return "0";
            }

        }

        public string ForMatNumber_SoLe(string input)
        {
            try
            {
                if (input.Trim().Length <= 0)
                    return "0";

                //return String.Format("{0:0,0.###}", double.Parse(input));
                return String.Format("{0:0.###;0.###;0}", double.Parse(input));
                //return double.Parse(input).ToString("0:0.#0", CultureInfo.InvariantCulture);      
            }
            catch (Exception e)
            {
                return "0";
            }

        }

        public string ForMatNumberBAOCAO(string input)
        {
            try
            {
                if (input.Trim().Length <= 0)
                    return "-";

                string kq = double.Parse(input).ToString("#,#", CultureInfo.InvariantCulture);
                if (kq.Trim().Length <= 0)
                    kq = "0";

                if (kq.Trim().Equals("0"))
                    kq = "-";

                return kq;
            }
            catch (Exception e)
            {
                return "-";
            }

        }


        public string ForMatNumberHOADON(string input)
        {
            try
            {
                if (input.Trim().Length <= 0)
                    return "0";

                string kq = double.Parse(input).ToString("#,#", CultureInfo.InvariantCulture);
                if (kq.Trim().Length <= 0)
                    kq = "0";

                if (kq.Contains("."))
                {
                    string[] arr = Regex.Split(kq, ".");
                    kq = Regex.Replace(arr[0], ",", "/.") + ", " + arr[1];
                }
                else
                    kq = Regex.Replace(kq, ",", ".");

                return kq;
            }
            catch (Exception e)
            {
                return "0";
            }

        }

        public string ForMatNumber_PL(string input)
        {
            try
            {
                if (input.Trim().Length <= 0)
                    return "";

                string kq = double.Parse(input).ToString("#,#", CultureInfo.InvariantCulture);
                if (kq.Trim().Length <= 0)
                    kq = "";

                return kq;
            }
            catch (Exception e)
            {
                return "";
            }

        }

        public string ForMatNumber_SoLeNEW(string input)
        {
            if (input.Trim().Length <= 0)
                return "0";

            if (!input.Contains("."))
                return double.Parse(input).ToString("#,#", CultureInfo.InvariantCulture);

            try
            {
                string[] data = Regex.Split(input, @"\.");

                string phanNGUYEN = double.Parse(data[0]).ToString("#,#", CultureInfo.InvariantCulture);
                if (phanNGUYEN.Trim().Length <= 0)
                    phanNGUYEN = "0";

                string phanle = "0";
                if (data[1].Trim().Length >= 3)
                    phanle = data[1].Substring(0, 3);
                else
                    phanle = data[1];

                while (phanle.EndsWith("0"))
                    phanle = phanle.Substring(0, phanle.Length - 1);

                if (phanle.Trim().Length > 0 && double.Parse(phanle) > 0)
                    return phanNGUYEN + "." + phanle;
                else
                    return phanNGUYEN;
            }
            catch (Exception e)
            {
                return "0";
            }
        }

        public string Change_AV(string ip_str_change)
        {
            ip_str_change = ip_str_change.Trim();

            Regex v_reg_regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string v_str_FormD = ip_str_change.Normalize(NormalizationForm.FormD);
            string kq = v_reg_regex.Replace(v_str_FormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');

            return Regex.Replace(kq.Trim(), " ", "-");
        }

        public string GetValue(string search, int pos)
        {
            try
            {
                string[] arr = Regex.Split(search, ";;");

                if (arr[pos].Contains("__"))
                {
                    string[] arr2 = Regex.Split(arr[pos], "__");
                    return arr2[1];
                }
            }
            catch (Exception e)
            {
                return "";
            }

            return "";

        }

        private string WriteToFile(string text)
        {
            string path = @"D:\Websites\POS\DatabaseLog.txt";
            try
            {
                using (StreamWriter writer = File.AppendText(path))
                {
                    writer.WriteLine(string.Format(text, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")));
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        public string GeneratePhone(string phone)
        {
            return Regex.Replace(phone, @"[^0-9]", "");
        }

        public string GenerateFormatPhone(string phone)
        {
            char[] array = phone.ToCharArray();
            string newPhone = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                {
                    newPhone += "(";
                }

                if (i == 6)
                {
                    newPhone += " ";
                }

                newPhone += array[i];

                if (i == 2)
                {
                    newPhone += ") ";
                }
            }

            return newPhone;
        }
        public bool IsNumber(string pText)
        {
            Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
            return regex.IsMatch(pText);
        }
    }
}
