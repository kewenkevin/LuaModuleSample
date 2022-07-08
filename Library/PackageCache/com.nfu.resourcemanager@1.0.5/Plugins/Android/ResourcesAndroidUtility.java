
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import com.unity3d.player.UnityPlayer;
import android.util.Log;
import android.content.Context;
import java.util.Vector;
import java.io.File;
import android.os.Environment;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import com.android.vending.expansion.zipfile.APKExpansionSupport;
import com.android.vending.expansion.zipfile.ZipResourceFile;
import java.io.IOException;

public class ResourcesAndroidUtility {

    public static int GetBundleVersionCode() throws PackageManager.NameNotFoundException {
        Context ctx = UnityPlayer.currentActivity;
        PackageManager pkgMgr = ctx.getPackageManager();
        PackageInfo pInfo = pkgMgr.getPackageInfo(ctx.getPackageName(), 0);
        return pInfo.versionCode;
    }

    public static byte[] ReadStreamingAssetsAllBytes (String path) {

        InputStream inputStream = null;

        try {

            inputStream = UnityPlayer.currentActivity.getAssets().open(path);
            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            byte buf[] = new byte[1024];
            int len;
            try {
                while ((len = inputStream.read(buf)) != -1) {
                    outputStream.write(buf, 0, len);
                }
                outputStream.close();
                inputStream.close();
    
            } catch (IOException e) {
    
            }
            return outputStream.toByteArray();
        } catch (IOException e) {

            Log.e("loadFile", e.getMessage());
            return null;
        }
    }


    public static byte[] ReadStreamingAssetsAllBytesWithObb(String path) {
        Context ctx = UnityPlayer.currentActivity;
        InputStream inputStream = null;

        try {
          
            int bundleVersionCode = GetBundleVersionCode();
            ZipResourceFile expansionFile = APKExpansionSupport.getAPKExpansionZipFile(ctx, bundleVersionCode, -1);
        
            if (expansionFile != null) {
                //FileDescriptor fd = expansionFile.getAssetFileDescriptor(path);
                //or
                inputStream = expansionFile.getInputStream("assets/"+ path);
                if(inputStream!=null){
       
                ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
                byte buf[] = new byte[1024];
                int len;

                while ((len = inputStream.read(buf)) != -1) {
                    outputStream.write(buf, 0, len);
                }
                outputStream.close();
                inputStream.close();

                return outputStream.toByteArray();
            }
            }
        } catch (IOException | PackageManager.NameNotFoundException e) {

            Log.e("loadFile", e.getMessage());
        }
        return null;
    }

    public static InputStream GetStreamingAssetsInputStreamWithObb(String path) {
        Context ctx = UnityPlayer.currentActivity;
        InputStream inputStream = null;

        try {
          
            int bundleVersionCode = GetBundleVersionCode();
            ZipResourceFile expansionFile = APKExpansionSupport.getAPKExpansionZipFile(ctx, bundleVersionCode, -1);
        
            if (expansionFile != null) {
                //FileDescriptor fd = expansionFile.getAssetFileDescriptor(path);
                //or
                inputStream = expansionFile.getInputStream("assets/"+ path);
                return inputStream;
            }
        } catch (IOException | PackageManager.NameNotFoundException e) {

            Log.e("loadFile", e.getMessage());
        }
        return null;
    }
}
