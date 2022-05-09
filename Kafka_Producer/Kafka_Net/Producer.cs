using KafkaNet;
using KafkaNet.Model;
using KafkaNet.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Confluent.Kafka;

namespace Kafka_Net
{
    public partial class Producer : Form
    {
        Uri uri = new Uri("http://localhost:9092");
        string topic = "";

        public Producer()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //txtTopic.Text = "TCGP1_in";
            topic = txtTopic.Text.TrimEnd();
            SendMessageToTopic(topic, txtMessage.Text.Trim());
            this.Clear();
        }
        public static async void SendMessageToTopic(string topic, string message)
        {
            var conf = new ProducerConfig {
                BootstrapServers = "localhost:9092,localhost:19092,localhost:29092",
                SecurityProtocol = SecurityProtocol.SaslPlaintext,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "TCGP4",
                SaslPassword = "TCGP4-secret"
            };
            using (var p = new ProducerBuilder<Null, string>(conf).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync(topic, new Message<Null, string> { Value = message });
                }
                catch (ProduceException<Null, string> e)
                {
                    MessageBox.Show($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
        void Clear()
        {
            this.txtMessage.Text = string.Empty;
            this.txtMessage.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
