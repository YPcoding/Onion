namespace Common.Helpers;

/// <summary>
/// 文件操作类
/// </summary>
public static class FileIOHelper
{
    /// <summary>
    /// 读取文本文件内容
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>文件内容字符串</returns>
    public static string ReadTextFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// 写入文本文件内容
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="content">要写入的内容</param>
    /// <returns>是否写入成功</returns>
    public static bool WriteTextFile(string filePath, string content)
    {
        try
        {
            File.WriteAllText(filePath, content);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 判断文件是否存在
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是否存在</returns>
    public static bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    /// <summary>
    /// 复制文件
    /// </summary>
    /// <param name="sourcePath">源文件路径</param>
    /// <param name="destinationPath">目标文件路径</param>
    /// <returns>是否复制成功</returns>
    public static bool CopyFile(string sourcePath, string destinationPath)
    {
        try
        {
            File.Copy(sourcePath, destinationPath);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是否删除成功</returns>
    public static bool DeleteFile(string filePath)
    {
        try
        {
            File.Delete(filePath);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 创建目录
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <returns>是否创建成功</returns>
    public static bool CreateDirectory(string directoryPath)
    {
        try
        {
            Directory.CreateDirectory(directoryPath);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 获取目录下的所有文件
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <returns>文件路径数组</returns>
    public static string[] GetFilesInDirectory(string directoryPath)
    {
        return Directory.GetFiles(directoryPath);
    }

    /// <summary>
    /// 获取目录下的所有子目录
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <returns>子目录路径数组</returns>
    public static string[] GetDirectoriesInDirectory(string directoryPath)
    {
        return Directory.GetDirectories(directoryPath);
    }

    /// <summary>
    /// 检查目录是否存在
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <returns>是否存在</returns>
    public static bool DirectoryExists(string directoryPath)
    {
        return Directory.Exists(directoryPath);
    }

    /// <summary>
    /// 删除目录及其内容
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <returns>是否删除成功</returns>
    public static bool DeleteDirectory(string directoryPath)
    {
        try
        {
            Directory.Delete(directoryPath, true);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 移动文件
    /// </summary>
    /// <param name="sourcePath">源文件路径</param>
    /// <param name="destinationPath">目标文件路径</param>
    /// <returns>是否移动成功</returns>
    public static bool MoveFile(string sourcePath, string destinationPath)
    {
        try
        {
            File.Move(sourcePath, destinationPath);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 读取二进制文件内容
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>二进制数据数组</returns>
    public static byte[] ReadBinaryFile(string filePath)
    {
        return File.ReadAllBytes(filePath);
    }

    /// <summary>
    /// 写入二进制文件内容
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="data">要写入的二进制数据</param>
    /// <returns>是否写入成功</returns>
    public static bool WriteBinaryFile(string filePath, byte[] data)
    {
        try
        {
            File.WriteAllBytes(filePath, data);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 追加文本到文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="content">要追加的文本内容</param>
    /// <returns>是否追加成功</returns>
    public static bool AppendTextToFile(string filePath, string content)
    {
        try
        {
            File.AppendAllText(filePath, content);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>文件信息对象</returns>
    public static FileInfo GetFileInfo(string filePath)
    {
        return new FileInfo(filePath);
    }

    /// <summary>
    /// 获取目录信息
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <returns>目录信息对象</returns>
    public static DirectoryInfo GetDirectoryInfo(string directoryPath)
    {
        return new DirectoryInfo(directoryPath);
    }

    /// <summary>
    /// 判断路径是否是文件
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>是否是文件</returns>
    public static bool IsFilePath(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// 判断路径是否是目录
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>是否是目录</returns>
    public static bool IsDirectoryPath(string path)
    {
        return Directory.Exists(path);
    }

    /// <summary>
    /// 从文件路径中获取文件名（带扩展名）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>文件名（带扩展名）</returns>
    public static string GetFileNameFromPath(string filePath)
    {
        return Path.GetFileName(filePath);
    }

    /// <summary>
    /// 从文件路径中获取文件名（不带扩展名）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>文件名（不带扩展名）</returns>
    public static string GetFileNameWithoutExtension(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath);
    }

    /// <summary>
    /// 从文件路径中获取文件扩展名
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>文件扩展名</returns>
    public static string GetFileExtension(string filePath)
    {
        return Path.GetExtension(filePath);
    }

    /// <summary>
    /// 合并目录和文件名为完整路径
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <param name="fileName">文件名</param>
    /// <returns>完整文件路径</returns>
    public static string CombinePath(string directoryPath, string fileName)
    {
        return Path.Combine(directoryPath, fileName);
    }

    /// <summary>
    /// 获取当前工作目录
    /// </summary>
    /// <returns>当前工作目录路径</returns>
    public static string GetCurrentDirectory()
    {
        return Environment.CurrentDirectory;
    }

    /// <summary>
    /// 设置当前工作目录
    /// </summary>
    /// <param name="directoryPath">要设置的工作目录路径</param>
    public static void SetCurrentDirectory(string directoryPath)
    {
        Environment.CurrentDirectory = directoryPath;
    }

    /// <summary>
    /// 获取特定环境目录路径（如应用程序数据目录、文档目录等）
    /// </summary>
    /// <param name="folder">特定环境目录枚举</param>
    /// <returns>特定环境目录路径</returns>
    public static string GetSpecialFolderPath(Environment.SpecialFolder folder)
    {
        return Environment.GetFolderPath(folder);
    }

    /// <summary>
    /// 检查文件是否为只读
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>文件是否为只读</returns>
    public static bool IsFileReadOnly(string filePath)
    {
        return (File.GetAttributes(filePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
    }

    /// <summary>
    /// 设置文件为只读
    /// </summary>
    /// <param name="filePath">文件路径</param>
    public static void SetFileReadOnly(string filePath)
    {
        File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.ReadOnly);
    }

    /// <summary>
    /// 设置文件为非只读
    /// </summary>
    /// <param name="filePath">文件路径</param>
    public static void SetFileNotReadOnly(string filePath)
    {
        File.SetAttributes(filePath, File.GetAttributes(filePath) & ~FileAttributes.ReadOnly);
    }

    /// <summary>
    /// 检查目录是否为只读
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <returns>目录是否为只读</returns>
    public static bool IsDirectoryReadOnly(string directoryPath)
    {
        return (File.GetAttributes(directoryPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
    }

    /// <summary>
    /// 设置目录为只读
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    public static void SetDirectoryReadOnly(string directoryPath)
    {
        File.SetAttributes(directoryPath, File.GetAttributes(directoryPath) | FileAttributes.ReadOnly);
    }

    /// <summary>
    /// 设置目录为非只读
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    public static void SetDirectoryNotReadOnly(string directoryPath)
    {
        File.SetAttributes(directoryPath, File.GetAttributes(directoryPath) & ~FileAttributes.ReadOnly);
    }

    /// <summary>
    /// 获取文件的大小（字节数）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>文件大小（字节数）</returns>
    public static long GetFileSize(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        return fileInfo.Length;
    }
}
