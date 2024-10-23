using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Drawing.Text;

namespace ychet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();      
        }

        DB db = new DB();

        private void Form1_Load(object sender, EventArgs e)
        {
            db.getConnection();
            SQLiteConnection connection = new SQLiteConnection(db.connection);
            connection.Open();  
            var command = new SQLiteCommand("SELECT id, name FROM location", connection);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                comboBox1.Items.Add(new { Text = reader["name"].ToString(), Value = reader["id"] });
            }
            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";

            SetupDataGridView();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try { 
                db.getConnection();

            if (comboBox1.Text != string.Empty)
            {
                var selectedRoom = (dynamic)comboBox1.SelectedItem;
                long roomID = selectedRoom.Value;

                dataGridView1.Rows.Clear();

                SQLiteConnection connection = new SQLiteConnection(db.connection);
                connection.Open();

                var command = new SQLiteCommand("SELECT name, quantity FROM technika WHERE location_id = @roomID", connection);
                command.Parameters.AddWithValue("@roomID", roomID);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataGridView1.Rows.Add(reader["name"].ToString(),
                                           reader["quantity"].ToString());

                }
            }
            else
            {
                MessageBox.Show("Выбирите кабинет");
            }
        }
            catch
            {
                MessageBox.Show("Выбирите кабинет");
            }
        }


        private void SetupDataGridView()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            dataGridView1.ColumnCount = 2;

            dataGridView1.Columns[0].Name = "Наименование";
            dataGridView1.Columns[1].Name = "Количество";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != string.Empty
               && textBox2.Text != string.Empty)
            {
                db.getConnection();
                SQLiteConnection connection = new SQLiteConnection(db.connection);        
                connection.Open();

                var selectedRoom = (dynamic)comboBox1.SelectedItem;
                long roomId = selectedRoom.Value;
                string Name = textBox1.Text;
                string quant = textBox2.Text;

                var command = new SQLiteCommand("INSERT INTO technika (name, quantity, location_id) VALUES (@name, @quant, @locationId)", connection);
                command.Parameters.AddWithValue("@name", Name);
                command.Parameters.AddWithValue("@quant", quant);
                command.Parameters.AddWithValue("@locationId", roomId);

                command.ExecuteNonQuery();

                LoadTech(roomId);
            }
            else
            {
                MessageBox.Show("Заполните данные");
            }
        }

        void LoadTech(long roomId)
        {
            dataGridView1.Rows.Clear();


            var selectedRoom = (dynamic)comboBox1.SelectedItem;
            long roomID = selectedRoom.Value;

            dataGridView1.Rows.Clear();

            SQLiteConnection connection = new SQLiteConnection(db.connection);
            connection.Open();

            var command = new SQLiteCommand("SELECT name, quantity FROM technika WHERE location_id = @roomID", connection);
            command.Parameters.AddWithValue("@roomID", roomID);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                dataGridView1.Rows.Add(reader["name"].ToString(),
                                       reader["quantity"].ToString());

            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                var selectedRoom = (dynamic)comboBox1.SelectedItem;

                long roomId = selectedRoom.Value;

                int rowIndex = e.RowIndex;

                string Name = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();

                int newQuant = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells[1].Value);

                db.getConnection();
                SQLiteConnection connection = new SQLiteConnection(db.connection);
                connection.Open();
                var command = new SQLiteCommand("UPDATE technika SET quantity = @newQuant WHERE name = @Name AND location_id = @roomId", connection);

                command.Parameters.AddWithValue("@newQuant", newQuant);
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@roomId", roomId);

                command.ExecuteNonQuery();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem != null && dataGridView1.SelectedRows.Count > 0)
            {
                db.getConnection();
                SQLiteConnection connection = new SQLiteConnection(db.connection);
                connection.Open();

                var selectedRoom = (dynamic)comboBox1.SelectedItem;
                long roomId = selectedRoom.Value;
                int selectedRow = dataGridView1.SelectedRows[0].Index;
                string Name = dataGridView1.Rows[selectedRow].Cells[0].Value.ToString();

                var command = new SQLiteCommand("DELETE FROM technika WHERE name = @name AND location_id = @roomId", connection);
                command.Parameters.AddWithValue("@name", Name);
                command.Parameters.AddWithValue("@roomId", roomId);

                int rowsAff = command.ExecuteNonQuery();
                if (rowsAff > 0)
                {
                    MessageBox.Show("Успешно удалено");
                    dataGridView1.Rows.RemoveAt(selectedRow);
                }
                else
                {
                    MessageBox.Show("Не удалось удалить технику");
                }
            }
            else
            {
                MessageBox.Show("Выберите кабинет и технику для удаления");
            }
        }
    }
}
