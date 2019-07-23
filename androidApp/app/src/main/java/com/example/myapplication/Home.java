package com.example.myapplication;

import androidx.appcompat.app.AppCompatActivity;
import android.app.Application;
import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.Socket;
public class Home extends AppCompatActivity {
    private Button syncButton, exitButton;
    private Context context;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_home);
        syncButton = findViewById(R.id.syncButton);
        exitButton = findViewById(R.id.exitButton);
        context = this;
        syncButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                new VsCodeReceiver().execute();
                syncButton.setEnabled(false);
            }
        });
    }

    private class VsCodeReceiver extends AsyncTask<String, String, String>
    {
        private boolean mRun = false;
        private String serverMessage;
        public static final String SERVERIP = "10.0.2.2"; //your computer IP address
        public static final int SERVERPORT = 5000;
        PrintWriter out;
        BufferedReader in;
        @Override
        protected String doInBackground(String... strings) {
            return this.Run();
        }
        @Override
        protected void onPostExecute(String result)
        {
            // content.setText(result);
            syncButton.setEnabled(true);
            Intent mainActivityIntent = new Intent(context, MainActivity.class);
            mainActivityIntent.putExtra("result", result);
            startActivity(mainActivityIntent);
        }
        public String Run()
        {
            mRun = true;
            try {
                //here you must put your computer's IP address.
                InetAddress serverAddr = InetAddress.getByName(SERVERIP);
                Log.e("TCP Client", "C: Connecting...");
                //create a socket to make the connection with the server
                Socket socket = new Socket(serverAddr, SERVERPORT);
                try {
                    //send the message to the server
                    out = new PrintWriter(new BufferedWriter(new OutputStreamWriter(socket.getOutputStream())), true);
                    Log.e("TCP Client", "C: Sent.");
                    out.print("Hello");
                    out.flush();
                    //receive the message which the server sends back
                    in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
                    //in this while the client listens for the messages sent by the server
                    StringBuilder builder = new StringBuilder();
                    while (mRun)
                    {
                        serverMessage = in.readLine();
                        if (serverMessage == null ||  serverMessage.contains("Exit")) {
                            //call the method messageReceived from MyActivity class
                            return builder.toString();
                        }
                        builder.append(serverMessage);
                        builder.append(System.getProperty("line.separator"));
                        serverMessage = null;
                    }
                    Log.e("RESPONSE FROM SERVER", "S: Received Message: '" + serverMessage + "'");
                } catch (Exception e) {
                    Log.e("TCP", "S: Error", e);
                } finally {
                    //the socket must be closed. It is not possible to reconnect to this socket
                    // after it is closed, which means a new socket instance has to be created.
                    socket.close();
                }
            } catch (Exception e) {
                Log.e("TCP", "C: Error", e);
            }
            return "Server error";
        }
    }

}