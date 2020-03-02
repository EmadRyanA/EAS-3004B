package com.DefaultCompany.NewUnityProject;
//This is for getting the result uri from the native android file

import com.unity3d.player.*;
import com.unity3d.player.UnityPlayerActivity;
import android.os.Bundle;
import android.util.Log;
import android.app.Activity;
import android.content.*;
import android.content.Intent;
import android.content.Context;
import android.net.Uri;
import android.database.Cursor;
import java.io.*;
import android.provider.OpenableColumns;
import android.provider.MediaStore.MediaColumns;
import android.provider.MediaStore;


public class CustomUnityPlayerActivity extends UnityPlayerActivity {

    public static final int REQUEST_CODE = 1;
    public static String outFilePath = "";
    
    //See https://docs.unity3d.com/Manual/AndroidUnityPlayerActivity.html
    protected void onCreate(Bundle savedInstanceState) {
        // call UnityPlayerActivity.onCreate()
        super.onCreate(savedInstanceState);
        // print debug message to logcat
        Log.d("CustomUnityPlayerActivity", "onCreate called!");
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        Log.d("CustomUnityPlayerActivity", "On Activty Result");
        if (requestCode == REQUEST_CODE) { //todo check for errors in intent
            Uri fileUri = data.getData();

            //https://developer.android.com/training/secure-file-sharing/retrieve-info
            //Cursor returnCursor = getContentResolver().query(fileUri, null, null, null, null);
            //int nameIndex = returnCursor.getColumnIndex(OpenableColumns.DISPLAY_NAME);
            //int sizeIndex = returnCursor.getColumnIndex(OpenableColumns.SIZE);
            //String fileName = returnCursor.getString(nameIndex);
            //long fileSize = returnCursor.getLong(sizeIndex);

            String[] projection = {
                MediaStore.Audio.Media.TITLE,
                MediaStore.Audio.Media.ARTIST,
                MediaStore.Audio.Media.DISPLAY_NAME
            };

            Cursor cursor = getContentResolver().query(fileUri, projection, null, null, null);

            cursor.moveToFirst();
            String songTitle = cursor.getString(0);
            String songArtist = cursor.getString(1);
            String songDisplayName = cursor.getString(2); 

            String filePath = outFilePath + "/" + songDisplayName; //todo: what if there are more than 1 song with name? Get some unique key

            Log.d("CustomUnityPlayerActivity", songDisplayName);

            //see https://commonsware.com/blog/2016/03/15/how-consume-content-uri.html
            try (InputStream in = getContentResolver().openInputStream(fileUri)) {
                //see https://stackoverflow.com/questions/9292954/how-to-make-a-copy-of-a-file-in-android/29685580
                try (OutputStream out = new FileOutputStream(filePath)) {
                  byte[] buf = new byte[1024];
                  int len;
                  while ((len = in.read(buf)) > 0) {
                      out.write(buf, 0, len);
                  }
                }
                catch (Exception e) {
                    Log.d("CustomUnityPlayerActivity", "E1");
                }
            }
            catch (Exception e) {
                Log.d("CustomUnityPlayerActivity", "E2");
            }

            UnityPlayer.UnitySendMessage("BGACanvas", "resultFromJava", filePath);
        }
        
    }

    public void CallFromUnity(String outFile) {

      outFilePath = outFile;
      Log.d("CustomUnityPlayerActivity", "I am called from unity");
      Intent intent = new Intent(Intent.ACTION_GET_CONTENT);
      intent.setType("audio/mpeg");
      //intent.addCategory(Intent.CATEGORY_OPENABLE);
      startActivityForResult(intent, REQUEST_CODE);
    }
}

