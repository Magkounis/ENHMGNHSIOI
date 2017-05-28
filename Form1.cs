using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace enhmgnhsioi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //loadtodatatable();
            //loattotableofkodans();
            //buildgnhsious();
        }
        DataTable neoapodt;
        DataTable distinctkodans;
        DataTable gnhsious;
        private void loadtodatatable()
        {
            button1.Focus();
                DataClass_1DataContext dc = new DataClass_1DataContext();

            var querry = from c in dc.NEOAPOs.Where(x => x.KODAN.ToString() != "")
                         select c;

          
             neoapodt= new DataTable();
          
           // create columns
          

           neoapodt.Columns.Add("KODAN");//create the two columns
           neoapodt.Columns.Add("NEWKODAN");
           int i=0;
            int max=querry.Count();
           foreach (var element in querry)//load querry to datatable
           {//for the elements in LINQUERRY
               var row = neoapodt.NewRow();//create new row in datatableneoapo
               row["KODAN"] = element.KODAN;
               row["NEWKODAN"] = element.NEWKODAN;//having kodan and newkodan
               neoapodt.Rows.Add(row);//and add them
               i++;
               progressBar1.Maximum =max ;
               progressBar1.Value = i;
           }
           
         
        }

        private void loattotableofkodans()
        {
            button2.Focus();
            DataClass_1DataContext dc = new DataClass_1DataContext();

            var querry = (from c in dc.NEOAPOs.Where(x => x.KODAN.ToString() != "")
                         select c.KODAN).Distinct();//i place parenthesis because i want 
            distinctkodans = new DataTable();      //to call distinct to the querry and not only to the string
           
            distinctkodans.Columns.Add("KODAN");
            int i = 0;
            int max = querry.Count();

            foreach (var element in querry)
            {
                var row = distinctkodans.NewRow();
                row["KODAN"] = element;
                distinctkodans.Rows.Add(row);
                i++;
                progressBar1.Maximum = max;
                progressBar1.Value = i;


            }


        }



        private void buildgnhsious()
        {
            button4.Focus();
            gnhsious = new DataTable();//create gnhsious table
            gnhsious.Columns.Add("KODAN");//Having kodan and gnhkodan
            gnhsious.Columns.Add("GNHKODAN");
            gnhsious.PrimaryKey = new DataColumn[] {gnhsious.Columns["KODAN"],   
                                         gnhsious.Columns["GNHKODAN"]};  
            

            object kodan1 = null;


            var querry=(from row in distinctkodans.AsEnumerable()//do select from costracted datatable
                        where row.Field<object>("KODAN")!=null//unique kodans each time
                        select row.Field<string>("KODAN"));

            progressBar1.Maximum=(from row in distinctkodans.AsEnumerable()//do count from costracted datatable
                        where row.Field<object>("KODAN")!=null
                        select row.Field<string>("KODAN")).Count();
            int i=0;

            foreach (var kodanelement in querry)//for each unique kodan
            {

                kodan1 = kodanelement.ToString().Trim();
                object kodan2 = kodan1;
                var result = "";
               
                while (result != null) 
                {

                  

                        result = (from row in neoapodt.AsEnumerable()   //as long as datatable is filled
                                  where row.Field<string>("KODAN").Trim() == kodan2.ToString().Trim()
                                    select row.Field<string>("NEWKODAN")).FirstOrDefault();
                                    //it is checking the newkodan chain


                    kodan2 = result;
                    if (kodan2 != null)
                    {                                      
                        DataRow newkodanRow = gnhsious.NewRow();//insert it to datatable gnhsiwn
                        newkodanRow["KODAN"] = kodan1;
                        newkodanRow["GNHKODAN"] = kodan2;
                        if ((!gnhsious.Rows.Contains(new object[] { kodan1, kodan2 })) && (kodan1.ToString().Trim() != "") && (kodan2.ToString().Trim() != ""))    //Contains(newkodanRow))         //returns a bool if already exists
                        {                                       //and also check if kodan1 isblack or kodan2 -newkodan is blank-empty
                            gnhsious.Rows.Add(newkodanRow); //i have to check at this stage if it allready exist 
                            //in data table gnhsiwn and if exeists then go brake the while loop
                            
                        }
                        else if ((!gnhsious.Rows.Contains(new object[] { kodan2, kodan1 })) && (kodan1.ToString().Trim() != "") && (kodan2.ToString().Trim() != ""))//also check for vice versa combo
                        {                                   //and also check if kodan1 isblack or kodan2 -newkodan is blank-empty
                            newkodanRow["KODAN"] = kodan2;
                            newkodanRow["GNHKODAN"] = kodan1;
                            gnhsious.Rows.Add(newkodanRow);

                        }
                        else
                        {
                            break;
                        }
                        newkodanRow = null;
                    }

                }

                i++;
                progressBar1.Value = i;//update a progress bar
                label1.Text = i.ToString();
            }

           
        }



        private void inserttognhsioustable()
        {
            button5.Focus();
            DataClass_1DataContext dc = new DataClass_1DataContext();

            progressBar1.Maximum = gnhsious.Rows.Count;
            int i = 0;
            foreach (DataRow row in gnhsious.Rows)
            {
                i++;
                GNHSIOI gnhsioitable = new GNHSIOI();
                
                gnhsioitable.KODAN = row.Field<string>("KODAN");
                gnhsioitable.GNHKODAN = row.Field<string>("GNHKODAN");
                gnhsioitable.MARKSOURCE = "";
                gnhsioitable.PRO = "G80";
                
                dc.GNHSIOIs.InsertOnSubmit(gnhsioitable);
                dc.SubmitChanges();
                progressBar1.Value = i;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadtodatatable();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loattotableofkodans();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            loadtodatatable();
            loattotableofkodans();
            buildgnhsious();
            inserttognhsioustable();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            buildgnhsious();
        }
    }
}
