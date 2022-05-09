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
using System.Collections;
using System.Linq;

namespace Kafka_Net
{

    public partial class Consumer : Form
    {
        IEnumerable<string> listTopics = new string[] { "TCGP1_in" };
        Boolean topicAdded = false;
        public Consumer()
        {
            InitializeComponent();
        }

        public void Start(ConsumerConfig consumerConfig)
        {
            List<string> list = listTopics.ToList();
            listTopics = (IEnumerable<string>)list;
            var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

            consumer.Subscribe(listTopics);
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    if (topicAdded == true)
                    {
                        list.Add(txtTopic.Text);
                        listTopics = (IEnumerable<string>)list;
                        consumer.Subscribe(listTopics);
                        topicAdded = false;
                        AddlstTopics(txtTopic.Text);
                    }
                }

            }).Start();
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    try
                    {
                        var mes = consumer.Consume();
                        //Console.WriteLine($"Message: {mes.Message.Value}");
                        showmessage(mes.Topic + " : " + mes.Message.Value);

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Delivery failed: {e.Message}");
                    }
                }

            }).Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092,localhost:19092,localhost:29092",
                GroupId = "test",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SecurityProtocol = SecurityProtocol.SaslPlaintext,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "admin",
                SaslPassword = "admin-secret",
            };
            Start(consumerConfig);
            CreatelstTopics();
        }
        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {

            if (this.txtMessage.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.txtMessage.AppendText(text);
                this.txtMessage.AppendText(Environment.NewLine);
            }
        }
        private void showmessage(string text)
        {
            SetText(text.ToString());
        }

        private void bAdd_Click(object sender, EventArgs e)
        {
            topicAdded = true;
        }

        private void AddlstTopics(string text)
        {
            if (this.lstTopics.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AddlstTopics);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.lstTopics.AppendText(text);
                this.lstTopics.AppendText(Environment.NewLine);
            }
        }
        private void CreatelstTopics()
        {
            foreach (string lsttopic in listTopics)
            {
                AddlstTopics(lsttopic);
            }
        }

    }
}
