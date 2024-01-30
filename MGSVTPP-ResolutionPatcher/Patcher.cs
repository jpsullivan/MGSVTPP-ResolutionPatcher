using Spectre.Console;

namespace MGSVTPP_ResolutionPatcher;

public class Patcher
{
    private readonly byte[] _resolutionPatch;
    private readonly string _installationPath;
    
    private static readonly byte[] PatchFind = [0x39, 0x8E, 0xE3, 0x3F];
    
    public Patcher(int resolutionX, int resolutionY)
    {
        _resolutionPatch = GetAspectRatioByteArrayFromResolution(resolutionX, resolutionY);
        _installationPath = AnsiConsole.Ask<string>("What is the local installation path for MGSVTPP?");
        
        // Check if patchReplace is the same length as PatchFind
        if (_resolutionPatch.Length != PatchFind.Length)
        {
            throw new Exception("Patch replacement size does not match the target size.");
        }
        
        PatchFile();
    }
    
    /// <summary>
    /// Assembles a byte array of the determined aspect ratio provided an X (width) and Y (height) value.
    /// </summary>
    /// <param name="resolutionX"></param>
    /// <param name="resolutionY"></param>
    /// <returns></returns>
    private static byte[] GetAspectRatioByteArrayFromResolution(int resolutionX, int resolutionY)
    {
        float aspectRatio = (float) resolutionX / resolutionY;
        byte[] aspectRatioInBytes = BitConverter.GetBytes(aspectRatio);

        return aspectRatioInBytes;
    }
    
    private void PatchFile()
    {

        var targetExecutablePath = Path.Join(_installationPath, "mgsvtpp.exe");
        if (!File.Exists(targetExecutablePath))
        {
            throw new Exception("Could not find the targeted executable 'mgsvtpp.exe'.");
        }

        var patchResult = PatchTheFile(targetExecutablePath, out var error);
        if (patchResult)
        {
            // we good
            return;
        }

        if (error != null)
        {
            throw new Exception("The file failed to patch!");
        }
    }
    
    private static bool DetectPatch(byte[] sequence, int position)
    {
        if (position + PatchFind.Length > sequence.Length)
        {
            return false;
        }
        
        return !PatchFind.Where((t, p) => t != sequence[position + p]).Any();
    }

    private bool PatchTheFile(string filename, out Exception error)
    {
        var foundValueForPatching = false;
        error = null;

        try
        {
            AnsiConsole.Write("Patching...");
            
            // Read file bytes.
            var fileContent = File.ReadAllBytes(filename);

            // Detect and patch file.
            for (int p = 0; p < fileContent.Length; p++)
            {
                if (!DetectPatch(fileContent, p))
                {
                    continue;
                }

                foundValueForPatching = true;
                for (int w = 0; w < PatchFind.Length; w++)
                {
                    fileContent[p + w] = _resolutionPatch[w];
                }
            }

            if (!foundValueForPatching)
            {
                return foundValueForPatching;
            }
            
            CreateBackup(filename);

            // Save it to another location.
            File.WriteAllBytes(filename, fileContent);

            return foundValueForPatching;
        }
        catch (Exception exception)
        {
            error = exception;
            return false;
        }
    }
    
    private void CreateBackup(string filename)
    {
        string dirPath = Path.GetDirectoryName(filename);
        string fileName = Path.GetFileName(filename);
        string backupExtension = ".backup";
        string[] files = Directory.GetFiles(dirPath);
        int count = files.Count(file => { return file.Contains(fileName + backupExtension); });
        string newFileName = (count == 0) ? filename + backupExtension : $"{fileName} ({count + 1}){backupExtension}";
        File.Copy(filename, newFileName);
    }
}