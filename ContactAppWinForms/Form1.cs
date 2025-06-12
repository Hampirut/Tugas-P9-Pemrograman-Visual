using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ContactAppWinForms
{
    public partial class Form1 : Form
    {
        string connectionString = "server=localhost;user id=root;password=;database=contactdb;";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Cek koneksi database
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("Koneksi ke database berhasil!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Koneksi ke database gagal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            LoadContacts();
        }

        private void LoadContacts()
        {
            dataGridViewContacts.Rows.Clear();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT name, email, phone, address FROM contacts";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        dataGridViewContacts.Rows.Add(
                            reader["name"].ToString(),
                            reader["email"].ToString(),
                            reader["phone"].ToString(),
                            reader["address"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat data: " + ex.Message);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Harap isi Nama, Email, dan Telepon!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "INSERT INTO contacts (name, email, phone, address) VALUES (@name, @email, @phone, @address)";
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                        cmd.ExecuteNonQuery();
                    }

                    LoadContacts();

                    txtName.Clear();
                    txtEmail.Clear();
                    txtPhone.Clear();
                    txtAddress.Clear();
                    txtName.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menyimpan ke database: " + ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewContacts.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridViewContacts.SelectedRows)
                {
                    string name = row.Cells[0].Value?.ToString();
                    if (!string.IsNullOrEmpty(name))
                    {
                        using (MySqlConnection conn = new MySqlConnection(connectionString))
                        {
                            try
                            {
                                conn.Open();
                                string query = "DELETE FROM contacts WHERE name = @name";
                                MySqlCommand cmd = new MySqlCommand(query, conn);
                                cmd.Parameters.AddWithValue("@name", name);
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Gagal menghapus data: " + ex.Message);
                            }
                        }
                    }
                }

                LoadContacts();
            }
            else
            {
                MessageBox.Show("Pilih baris yang ingin dihapus!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
