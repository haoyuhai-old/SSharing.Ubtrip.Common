//using CompressedViewState.Compression;
using System;
using System.ComponentModel;
using System.Web.UI;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace SSharing.Ubtrip.Common.Util.ViewStateCompress
{
    #region Page类
    /// <summary>
    /// ViewState的压缩类
    /// </summary>
    public class ViewStateCompress_Page : System.Web.UI.Page
    {
        //private CompressionLevels _compressionLevel = CompressionLevels.DefaultCompression;
        //private System.Web.UI.PageStatePersister _persister;

        ///// <summary>
        ///// ViewState压缩级别
        ///// </summary>
        //[Browsable(false)]
        //public CompressionLevels ViewStateCompressionLevel
        //{
        //    get
        //    {
        //        return this._compressionLevel;
        //    }
        //    set
        //    {
        //        this._compressionLevel = value;
        //    }
        //}

        //protected override System.Web.UI.PageStatePersister PageStatePersister
        //{
        //    get
        //    {   
        //        if (this._persister == null)
        //        {
        //            this._persister = new ZipCompressionPageStatePersister(this);
        //        }
        //        return this._persister;
        //    }
        //}
    }
    #endregion

    //#region ViewState处理
    //internal class ZipCompressionPageStatePersister : PageStatePersister
    //{
    //    private ViewStateCompress_Page _compressedPage;
    //    private static LosFormatter _formatter = new LosFormatter();
    //    private CompressionLevels _level;
    //    private const string LevelKey = "__COMPLVL";
    //    private const string StateKey = "____VIEWSTATE";

    //    public ZipCompressionPageStatePersister(System.Web.UI.Page page)
    //        : base(page)
    //    {
    //        this._level = CompressionLevels.DefaultCompression;
    //        this._compressedPage = page as ViewStateCompress_Page;
    //    }

    //    public override void Load()
    //    {
    //        CompressionLevels noCompression = CompressionLevels.NoCompression;
    //        if (base.Page.Request.Form["__COMPLVL"] != null)
    //        {
    //            noCompression = (CompressionLevels)int.Parse(base.Page.Request.Form["__COMPLVL"]);
    //        }
    //        this._compressedPage.ViewStateCompressionLevel = noCompression;
    //        string str = base.Page.Request.Form["____VIEWSTATE"];
    //        if (!string.IsNullOrEmpty(str))
    //        {
    //            Pair pair;
    //            if (noCompression == CompressionLevels.NoCompression)
    //            {
    //                pair = (Pair)_formatter.Deserialize(str);
    //            }
    //            else
    //            {
    //                pair = (Pair)ZipCompression.Decompress(str, this._level);
    //            }
    //            if (!base.Page.EnableViewState)
    //            {
    //                base.ViewState = null;
    //            }
    //            else
    //            {
    //                base.ViewState = pair.First;
    //            }
    //            base.ControlState = pair.Second;
    //        }
    //    }

    //    public override void Save()
    //    {
    //        base.Page.ClientScript.RegisterHiddenField("__COMPLVL", ((int)this._compressedPage.ViewStateCompressionLevel).ToString());
    //        if (!base.Page.EnableViewState)
    //        {
    //            base.ViewState = null;
    //        }
    //        if ((base.ViewState != null) || (base.ControlState != null))
    //        {
    //            string str;
    //            Pair pair = new Pair(base.ViewState, base.ControlState);
    //            if (this._compressedPage.ViewStateCompressionLevel == CompressionLevels.NoCompression)
    //            {
    //                StringWriter output = new StringWriter();
    //                _formatter.Serialize(output, pair);
    //                str = output.ToString();
    //                output.Flush();
    //                output.Close();
    //            }
    //            else
    //            {
    //                str = ZipCompression.Compress(pair, this._compressedPage.ViewStateCompressionLevel);
    //            }
    //            base.Page.ClientScript.RegisterHiddenField("____VIEWSTATE", str);
    //        }
    //    }
    //}
    //#endregion

    //#region 压缩处理
    //internal static class ZipCompression
    //{
    //    private static LosFormatter _formatter = new LosFormatter();

    //    public static string Compress(object obj, CompressionLevels lvl)
    //    {
    //        StringWriter output = new StringWriter();
    //        _formatter.Serialize(output, obj);
    //        byte[] buffer = Convert.FromBase64String(output.ToString());
    //        output.Flush();
    //        output.Close();
    //        MemoryStream baseOutputStream = new MemoryStream();
    //        DeflaterOutputStream stream2 = new DeflaterOutputStream(baseOutputStream, new Deflater(GetCompressionLevel(lvl)), 0x20000);
    //        stream2.Write(buffer, 0, buffer.Length);
    //        stream2.Close();
    //        string str = Convert.ToBase64String(baseOutputStream.ToArray());
    //        baseOutputStream.Close();
    //        return str;
    //    }

    //    public static object Decompress(string stateString, CompressionLevels lvl)
    //    {
    //        byte[] buffer = Convert.FromBase64String(stateString);
    //        MemoryStream stream = new MemoryStream();
    //        InflaterInputStream stream2 = new InflaterInputStream(new MemoryStream(buffer));
    //        byte[] buffer2 = new byte[0x1000];
    //        while (true)
    //        {
    //            int count = stream2.Read(buffer2, 0, buffer2.Length);
    //            if (count <= 0)
    //            {
    //                break;
    //            }
    //            stream.Write(buffer2, 0, count);
    //        }
    //        stream2.Close();
    //        stream.Position = 0L;
    //        string input = Convert.ToBase64String(stream.ToArray());
    //        object obj2 = _formatter.Deserialize(input);
    //        stream.Close();
    //        return obj2;
    //    }

    //    public static int GetCompressionLevel(CompressionLevels lvl)
    //    {
    //        switch (lvl)
    //        {
    //            case CompressionLevels.BestCompression:
    //                return Deflater.BEST_COMPRESSION;

    //            case CompressionLevels.Deflated:
    //                return Deflater.DEFLATED;

    //            case CompressionLevels.DefaultCompression:
    //                return Deflater.DEFAULT_COMPRESSION;

    //            case CompressionLevels.BestSpeed:
    //                return Deflater.BEST_SPEED;
    //        }
    //        return Deflater.NO_COMPRESSION;
    //    }
    //}
    //#endregion

    ///// <summary>
    ///// 压缩级别枚举
    ///// </summary>
    //public enum CompressionLevels
    //{
    //    /// <summary>
    //    /// 最好的压缩
    //    /// </summary>
    //    BestCompression,
    //    /// <summary>
    //    /// 较好的压缩
    //    /// </summary>
    //    Deflated,
    //    /// <summary>
    //    /// 标准的压缩
    //    /// </summary>
    //    DefaultCompression,
    //    /// <summary>
    //    /// 最快的压缩
    //    /// </summary>
    //    BestSpeed,
    //    /// <summary>
    //    /// 无压缩
    //    /// </summary>
    //    NoCompression
    //}
}
