using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab5
{
    public partial class Form1 : Form
    {
        SqlConnection conn;
        DataSet ds = new DataSet();
        DataTable tablePerson = new DataTable();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String str = "Server=DESKTOP-7TI187F;DataBase=School; Integrated Security=true;";
            conn = new SqlConnection(str);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            conn.Open();
            String sql = "SELECT * FROM Person";

            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;

            //Llenamos el dataSet con una tabla llamada Person
            adapter.Fill(ds, "Person");

            //Asignamos esa tabla del dataset a un objeto Table
            //para trabajar directamente con el
            tablePerson = ds.Tables["Person"];


            //SqlDataReader reader = cmd.ExecuteReader();
            //DataTable dt = new DataTable();
            //dt.Load(reader);

            dvgListado.DataSource = tablePerson;
            dvgListado.Update();
            conn.Close();
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("InsertPerson", conn);

            if (txtHireDate.Text == "")
                txtHireDate.Text = null;
            if (txtEnrollmentDate.Text == "")
                txtEnrollmentDate = null;

            cmd.Parameters.AddWithValue("@FirstName", txtNombre);
            cmd.Parameters.AddWithValue("@LastName", txtApellido);
            cmd.Parameters.AddWithValue("@HireDate", txtHireDate);
            cmd.Parameters.AddWithValue("@EnrollmentDate", txtEnrollmentDate);

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.InsertCommand = cmd;
            adapter.InsertCommand.CommandType = CommandType.StoredProcedure;

            DataRow row = tablePerson.NewRow();
            row["LastName"] = txtApellido.Text;
            row["FirstName"] = txtNombre.Text;
            row["HireDate"] = txtHireDate.Text;
            row["EnrollmentDate"] = txtEnrollmentDate.Text;

            tablePerson.Rows.Add(row);
            adapter.Update(tablePerson);

            int codigo = Convert.ToInt32(cmd.ExecuteScalar());

            MessageBox.Show("Se ha registrado nueva persona con el codigo" + codigo);
            conn.Close();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            conn.Open();            
            SqlCommand cmd = new SqlCommand("UpdatePerson", conn);                        

            cmd.Parameters.AddWithValue("@FirstName", txtNombre);
            cmd.Parameters.AddWithValue("@LastName", txtApellido);
            cmd.Parameters.AddWithValue("@HireDate", txtEnrollmentDate);
            cmd.Parameters.AddWithValue("@EnrollmentDate", txtEnrollmentDate);

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.UpdateCommand = cmd;
            adapter.UpdateCommand.CommandType = CommandType.StoredProcedure;

            DataRow[] fila = tablePerson.Select("PersonID='"+txtCodigo.Text+"'");
            fila[0]["LastName"] = txtApellido.Text;
            fila[1]["FirstName"] = txtNombre.Text;
            fila[2]["HireDate"] = txtHireDate.Text;
            fila[3]["EnrollmentDate"] = txtEnrollmentDate.Text;

            adapter.Update(tablePerson);

            int resultado = cmd.ExecuteNonQuery();

            if (resultado > 0)
            {
                MessageBox.Show("Se ha modificado el registro correctamente");

            }
            conn.Close();

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            conn.Open();
            String sp = "DeletePerson";
            SqlCommand cmd = new SqlCommand(sp, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PersonID", txtCodigo);

            int resultado = cmd.ExecuteNonQuery();

            if (resultado > 0)
            {
                MessageBox.Show("Se ha eliminado el registro correctamente");
            }
            conn.Close();
        }

        private void dvgListado_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dvgListado_SelectionChanged(object sender, EventArgs e)
        {
            if (dvgListado.SelectedRows.Count > 0)
            {
                txtCodigo.Text = dvgListado.SelectedRows[0].Cells[0].Value.ToString();
                txtNombre.Text = dvgListado.SelectedRows[0].Cells[1].Value.ToString();
                txtApellido.Text = dvgListado.SelectedRows[0].Cells[2].Value.ToString();
                
                string hireDate = dvgListado.SelectedRows [0].Cells[3].Value.ToString();
                if(String.IsNullOrEmpty(hireDate))
                    txtHireDate.Checked = false;
                else
                    txtHireDate.Text = hireDate;

                string enrollmentDate = dvgListado.SelectedRows[0].Cells [4].Value.ToString();
                if(String.IsNullOrEmpty(enrollmentDate))
                    txtEnrollmentDate.Checked = false;
                else
                    txtEnrollmentDate.Text = enrollmentDate;                
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                String FirstName = txtNombre.Text;

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "GetPersonNombre";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@FirstName";
                param.SqlDbType = SqlDbType.NVarChar;
                param.Value = FirstName;

                cmd.Parameters.Add(param);

                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                dvgListado.DataSource = dt;
                dvgListado.Refresh();
            }
            else
            {
                MessageBox.Show("La conexion esta cerrada");
            }
            conn.Close();
        }

        private void btnApellido_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(tablePerson);
            dv.Sort = "LastName ASC";
            dvgListado.DataSource= dv;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(tablePerson);
            dv.RowFilter = "PersonID= '" + txtCodigo.Text + "'";
            dvgListado.DataSource = dv;
        }
    }
}
