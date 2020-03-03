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
import android.media.MediaMetadataRetriever;
import android.graphics.BitmapFactory;
import android.graphics.Bitmap;
import android.graphics.Bitmap.CompressFormat;


public class CustomUnityPlayerActivity extends UnityPlayerActivity {

    public static final int REQUEST_CODE = 1;
    public static String outFilePath = "";
    public static final char delimeter = '~';
    
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

            //String[] projection = {
            //    MediaStore.Audio.Media.TITLE,
            //    MediaStore.Audio.Media.ARTIST,
            //    MediaStore.Audio.Media.DISPLAY_NAME
            //};

            //Cursor cursor = getContentResolver().query(fileUri, projection, null, null, null);

            //cursor.moveToFirst();
            //String songTitle = (cursor.getString(0) == null ? "NA" : cursor.getString(0)).replaceAll("~", "");
            //String songArtist = (cursor.getString(1) == null ? "NA" : cursor.getString(1)).replaceAll("~", "");
            //String songDisplayName = (cursor.getString(2) == null ? "NA" : cursor.getString(2)).replaceAll("~", "");

            MediaMetadataRetriever metaData = new MediaMetadataRetriever();
            metaData.setDataSource(this, fileUri);

            String songTitle = metaData.extractMetadata(MediaMetadataRetriever.METADATA_KEY_TITLE);
            String songArtist = metaData.extractMetadata(MediaMetadataRetriever.METADATA_KEY_ARTIST);
            String songAlbum = metaData.extractMetadata(MediaMetadataRetriever.METADATA_KEY_ALBUM);
            byte[] songArt = metaData.getEmbeddedPicture();
            Bitmap songBitmap = (songArt == null) ? null : BitmapFactory.decodeByteArray(songArt, 0, songArt.length);
            songTitle = (songTitle == null ? "NA" : songTitle).replaceAll("~", "");
            songArtist = (songArtist == null ? "NA": songArtist).replaceAll("~", "");
            songAlbum = (songAlbum == null ? "NA": songAlbum).replaceAll("~", "");

            String songInfo = (songTitle + "~" + songArtist + "~" + songAlbum);

            String filePath = outFilePath + "/" + songInfo; //todo this is still not perfect...

            if (songBitmap != null) {
                try (OutputStream out = new FileOutputStream(filePath + ".png")) {
                    songBitmap.compress(Bitmap.CompressFormat.PNG, 100, out);
                }
                catch (Exception e) {

                }
            }

            Log.d("CustomUnityPlayerActivity", filePath);

            //see https://commonsware.com/blog/2016/03/15/how-consume-content-uri.html
            try (InputStream in = getContentResolver().openInputStream(fileUri)) {
                //see https://stackoverflow.com/questions/9292954/how-to-make-a-copy-of-a-file-in-android/29685580
                try (OutputStream out = new FileOutputStream(filePath + ".mp3")) {
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

            UnityPlayer.UnitySendMessage("BGACanvas", "resultFromJava", songInfo);
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

