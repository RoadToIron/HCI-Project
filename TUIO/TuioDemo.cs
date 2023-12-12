/*
	TUIO C# Demo - part of the reacTIVision project
	Copyright (c) 2005-2016 Martin Kaltenbrunner <martin@tuio.org>

	This program is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using TUIO;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;

// gesture = "";



public class TuioDemo : Form , TuioListener
	{

    int byteCount;
    NetworkStream stream;
    byte[] sendData;
    TcpClient tcpClient;

	int gesture = -1;
	string data = "";
    int i = 0;
	

    public bool connectToSocket(String host, int portNumber)
    {
        try
        {
            tcpClient = new TcpClient(host, portNumber);
            stream = tcpClient.GetStream();
            MessageBox.Show("Connection Made ! with " + host + portNumber);
            return true;
        }
        catch (System.Net.Sockets.SocketException e)
        {
            MessageBox.Show("Connection Failed: " + e);
            return false;
        }
    }
    //private string gesture;

    public bool RecieveMessage()
    {
        try
        {
            byte[] receieveBuffer = new byte[1024];
            if (stream != null)
            {
                int byteReceived = stream.Read(receieveBuffer, 0, 1024);
                while (true)
                {
                    string data = Encoding.UTF8.GetString(receieveBuffer);
                    //byteReceived = stream.Read(receieveBuffer, 0, 1024);
                    gesture = Int16.Parse(data);
                    if (gesture == 1)
                    {
                        testflag = true;
                    }
                } 
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    public bool closeConnection()
    {
        if (stream != null)
        {
            stream.Close();
            tcpClient.Close();
            Console.WriteLine("Connection terminated ");
            return true;
        }
        else
        {
            return false;
        }
    }
    int flagcol1 = 0;
	int flagcol2 = 0;
	int flaghappened1 = -1;
	int flaghappened2= -1;
	int flaghappened3 = 0;
    bool testflag = false;
    private TuioClient client;
	private Dictionary<long,TuioObject> objectList;
	private Dictionary<long,TuioCursor> cursorList;
	private Dictionary<long,TuioBlob> blobList;




    private bool fullscreen;
	private bool verbose;

	Font font = new Font("Arial", 10.0f);
		
	int flag = 0;

		public TuioDemo(int port) {
		
			verbose = false;
			

			this.ClientSize = new System.Drawing.Size(500, 500);
			this.Name = "TuioDemo";
			this.Text = "TuioDemo";
			
			this.Closing+=new CancelEventHandler(Form_Closing);
			

			this.SetStyle( ControlStyles.AllPaintingInWmPaint |
							ControlStyles.UserPaint |
							ControlStyles.DoubleBuffer, true);

			objectList = new Dictionary<long,TuioObject>(128);
			cursorList = new Dictionary<long,TuioCursor>(128);
			blobList   = new Dictionary<long,TuioBlob>(128);
			
			client = new TuioClient(port);
			client.addTuioListener(this);

			client.connect();
		}

		

		private void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			client.removeTuioListener(this);

			client.disconnect();
			System.Environment.Exit(0);
		}
    public void addTuioObject(TuioObject o) {
        lock (objectList) {
				objectList.Add(o.SessionID,o);
			} if (verbose) Console.WriteLine("add obj "+o.SymbolID+" ("+o.SessionID+") "+o.X+" "+o.Y+" "+o.Angle);
		//mine
        if (o.SymbolID==11)
        {
			flaghappened2 = 0;
			flaghappened1 = 1;
			flagcol2 = 0;
        }
		if (o.SymbolID == 10)
		{
			flagcol1 = 0;
			flaghappened2 = 1;
			flaghappened1 = 0;
			
		}
		//mine
	}

		public void updateTuioObject(TuioObject o) {

			if (verbose) Console.WriteLine("set obj "+o.SymbolID+" "+o.SessionID+" "+o.X+" "+o.Y+" "+o.Angle+" "+o.MotionSpeed+" "+o.RotationSpeed+" "+o.MotionAccel+" "+o.RotationAccel);
		}

		public void removeTuioObject(TuioObject o) {
			lock(objectList) {
				objectList.Remove(o.SessionID);
			}
			if (verbose) Console.WriteLine("del obj "+o.SymbolID+" ("+o.SessionID+")");
		}

		public void addTuioCursor(TuioCursor c) {
			lock(cursorList) {
				cursorList.Add(c.SessionID,c);
			}
			if (verbose) Console.WriteLine("add cur "+c.CursorID + " ("+c.SessionID+") "+c.X+" "+c.Y);
		}

		public void updateTuioCursor(TuioCursor c) {
		if (verbose)
		{
			Console.WriteLine("set cur " + c.CursorID + " (" + c.SessionID + ") " + c.X + " " + c.Y + " " + c.MotionSpeed + " " + c.MotionAccel);
		}
		}

		public void removeTuioCursor(TuioCursor c) {
			lock(cursorList) {
				cursorList.Remove(c.SessionID);
			}
			if (verbose) Console.WriteLine("del cur "+c.CursorID + " ("+c.SessionID+")");
 		}

		public void addTuioBlob(TuioBlob b) {
			lock(blobList) {
				blobList.Add(b.SessionID,b);
			}
			if (verbose) Console.WriteLine("add blb "+b.BlobID + " ("+b.SessionID+") "+b.X+" "+b.Y+" "+b.Angle+" "+b.Width+" "+b.Height+" "+b.Area);
		}

		public void updateTuioBlob(TuioBlob b) {
		
			if (verbose) Console.WriteLine("set blb "+b.BlobID + " ("+b.SessionID+") "+b.X+" "+b.Y+" "+b.Angle+" "+b.Width+" "+b.Height+" "+b.Area+" "+b.MotionSpeed+" "+b.RotationSpeed+" "+b.MotionAccel+" "+b.RotationAccel);
		}

		public void removeTuioBlob(TuioBlob b) {
			lock(blobList) {
				blobList.Remove(b.SessionID);
			}
			if (verbose) Console.WriteLine("del blb "+b.BlobID + " ("+b.SessionID+")");
		}

		public void refresh(TuioTime frameTime) {
			Invalidate();
		}
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{

        // Getting the graphics object
        Graphics g = pevent.Graphics;
		g.Clear(Color.Purple);
        Int16 var1 = 0;
        Int16 var2 = 1;


		/*if (flaghappened1==1)
        {
            
            g.Clear(Color.Red);

            if (flagcol1 == 0)
			{
                g.Clear(Color.Red);
				flagcol1 = 20;
            }
            if (flagcol1 == 1)
			{
                g.Clear(Color.Blue);
                flagcol1 = 20;
            }
		}
		if (flaghappened2 == 1)
		{
            if (flagcol2 == 0)
			{
				g.Clear(Color.Green);
			}
			if (flagcol2 == 1)
			{
				g.Clear(Color.Black);
			}
		}*/


		if (flaghappened1 == 1)
		{
			g.Clear(Color.Red);
			if (gesture == var1)
			{
				g.Clear(Color.Red);

			}
			//MessageBox.Show(gesture + var2);
			if (gesture == var2)
			{
				g.Clear(Color.Beige);

			}

		}
		else if (flaghappened1 == 0)
		{
           
            if (gesture == var1)
			{
                g.Clear(Color.Green);
                //flagcol2 = 1;

            }
			if (gesture == var2)
			{
				g.Clear(Color.Black);
				//MessageBox.Show("black");
				//flagcol2 = 0;
				
			}
		}
	}
    public static void Main(String[] argv) {

        int port = 0;
			switch (argv.Length) {
				case 1:
					port = int.Parse(argv[0],null);
					if(port==0) goto default;
					break;
				case 0:
					port = 3333;
					break;
				default:
					Console.WriteLine("usage: mono TuioDemo [port]");
					System.Environment.Exit(0);
					break;
			}
        TuioDemo app = new TuioDemo(port);

        Thread thread = new Thread(new ThreadStart(() =>
			{
				app.connectToSocket("localhost", 3000);
                app.RecieveMessage();
                app.closeConnection();
			}));
			thread.Start();
			Application.Run(app);
		}

}
