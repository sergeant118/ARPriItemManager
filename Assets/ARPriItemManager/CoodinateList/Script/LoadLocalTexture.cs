using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoadLocalTexture : SingletonMonoBehaviour<LoadLocalTexture>
{
    private Texture2D m_texture;
    private byte[] texture_bytes;

    private Dictionary<string, Dictionary<string, Texture2D>> spriteListCache = new Dictionary<string, Dictionary<string, Texture2D>>();

    private bool is_init = false;

    // Use this for initialization.
    void Start()
    {
        generateDummyImageByte();

        string channel_folder_path = Application.persistentDataPath + "/ItemImage/";

        Debug.Log(channel_folder_path);

        //var path = DirectoryUtils.SafeCreateDirectory(channel_folder_path);
        DirectoryUtils.SafeCreateDirectory(channel_folder_path);


        /*
        string[] path_array = Directory.GetDirectories(channel_folder_path);

        List<Coroutine> coroutines = new List<Coroutine>();

        int array_num = path_array.Length;
        for (int i = 0; i < array_num; i++)
        {
            //Debug.Log(path_array[i] + ":" + Path.GetFileName(path_array[i]));

            Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();
 
            spriteListCache.Add(Path.GetFileName(path_array[i]), images);

            coroutines.Add( StartCoroutine(Texture2DListFromFolder(path_array[i], images)) );
        }

        

        foreach (Coroutine co in coroutines)
            yield return co;

        */

        is_init = true;

        //Debug.Log("すべての読み込みが完了しました。");


        

        //JSONの読み込みを待つ
        //while (!LoadJSONfromGAS.Instance.isInit())
        //    yield return null;

        //JSONに記載されているチャンネルのリスト
        //List<string> channel_list = LoadJSONfromGAS.Instance.getChannelList();

        //読み込んだJSON
        //ItemListJSON ILJ = LoadJSONfromGAS.Instance.current_ilj;

        //initLocalTexture(channel_list, ILJ);
        //Debug.Log("追加画像の読み込み完了");

        //Debug.Log("すべての読み込みが完了しました。");
    }

    public bool isInit()
    {
        return is_init;
    }

    public void initLocalTexture(List<string> channel_list, ItemListJSON　ijl)
    {
        foreach (string channel_id in channel_list)
        {
            getChannelImageTexture2D(channel_id, ijl);
        }
    }

    /// <summary>
    /// アイテム番号からTexture2Dを取得する
    /// </summary>
    /// <param name="channel_id"></param>
    /// <param name="image_name"></param>
    /// <returns></returns>
    public Texture2D getTexture2D(string channel_id, string image_name)
    {

        string channel_folder_path = Application.persistentDataPath + "/ItemImage/" + channel_id;

        string filePath = channel_folder_path + "/" + image_name + ".jpg";

        if (File.Exists(filePath))
        {
            //ファイルパスからjpg読み込み
            Texture2D sprite = Texture2DFromFile(filePath);

            //Debug.Log("Direct Load:" + filePath);

            return sprite;
        }
        else
        {
            //Debug.Log("return channnel dummy");
            return m_texture;
        }
        
        /*
        if (!spriteListCache.ContainsKey(channel_id)) {
            //まだキャッシュに無いときは暫定的に直接読み込みに行く

            string channel_folder_path = Application.persistentDataPath + "/ItemImage/" + channel_id;

            string filePath = channel_folder_path + "/" + image_name + ".jpg";

            if (File.Exists(filePath))
            {
                //ファイルパスからjpg読み込み
                Texture2D sprite = Texture2DFromFile(filePath);

                //Debug.Log("Direct Load:" + filePath);

                return sprite;
            }
            else
            {
                //Debug.Log("return channnel dummy");
                return m_texture;
            }
        }

        if (!spriteListCache[channel_id].ContainsKey(image_name))
        {
            string channel_folder_path = Application.persistentDataPath + "/ItemImage/" + channel_id;

            string filePath = channel_folder_path + "/" + image_name + ".jpg";

            if (File.Exists(filePath))
            {
                //ファイルパスからjpg読み込み
                Texture2D sprite = Texture2DFromFile(filePath);

                //Debug.Log("Direct Load:" + filePath);

                return sprite;
            }
            else
            {

                //Debug.Log(channel_id + ":" + image_name + " return image dummy");
                return m_texture;
            }
        }
        

        return spriteListCache[channel_id][image_name];

        */
    }

    public Texture2D getTexture2D(string image_name)
    {

        string channel_folder_path = Application.persistentDataPath + "/ItemImage";

        string filePath = channel_folder_path + "/" + image_name + ".jpg";

        Debug.Log("Loading:" + filePath);

        if (File.Exists(filePath))
        {
            //ファイルパスからjpg読み込み
            Texture2D sprite = Texture2DFromFile(filePath);

            //Debug.Log("Direct Load:" + filePath);

            return sprite;
        }
        else
        {
            //Debug.Log("return channnel dummy");
            return m_texture;
        }

    }

    public void setTexture2D(ContentManager cm, string image_name)
    {
        string channel_folder_path = Application.persistentDataPath + "/ItemImage";

        string filePath = channel_folder_path + "/" + image_name + ".jpg";

        if (File.Exists(filePath))
        {

            //cm.refleshImage();


            //ファイルパスからjpg読み込み
            Texture2D l_texture = Texture2DFromFile(filePath);
            cm.setImage(l_texture);

            //cm.setItem

            //image.texture = l_texture;
        }
        else
        {
            cm.setImage(m_texture);

            PUFirebaseTwitterLogin.Instance.storageTest(cm, image_name);

            //image.texture = m_texture;
        }

    }

    public void setDummyImage(ref RawImage image)
    {
        image.texture = m_texture;
    }


    private void getChannelImageTexture2D(string channel_id, ItemListJSON ijl)
    {
        Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();

        string channel_folder_path = Application.persistentDataPath + "/ItemImage/" + channel_id;

        //pathがなければ作成
        var path = DirectoryUtils.SafeCreateDirectory(channel_folder_path);

        if (path == null)
        {
            //ディレクトリが見つかったら

            //既にチャンネル登録されていたらそっちから呼び出し
            if (spriteListCache.ContainsKey(channel_id))
            {
                images = spriteListCache[channel_id];
            }

            //ここループ
            foreach (ItemJSON item in ijl.item_list)
            {

                if (item.database_name == channel_id)
                {
                    //ファイルの存在チェック
                    //string image_name = "70400";
                    string image_name = item.image_num.ToString();
                    string image_path = channel_folder_path + "/" + image_name + ".jpg";

                    //Dictionaryに含まれていない場合
                    if (!images.ContainsKey(image_name))
                    {
                        //ファイルの存在チェック
                        if (File.Exists(image_path))
                        {
                            //Debug.Log(image_name + " is exist");
                        }
                        else
                        {
                            //なければダミーを追加
                            saveDummyJpg(image_path);
                            //Debug.Log(image_name + " is generated");
                        }

                        //ファイルパスからjpg読み込み
                        //Sprite sprite = SpriteFromFile(image_path);
                        Texture2D sprite = Texture2DFromFile(image_path);
                        sprite.name = image_name + ".jpg";

                        images.Add(image_name, sprite);
                    }
                }
            }
        }
        else
        {
            //ここループ
            foreach (ItemJSON item in ijl.item_list)
            {
                if (item.database_name == channel_id)
                {
                    //ディレクトリが見つからなかったらダミー画像を生成
                    string image_name = item.image_num.ToString();
                    string image_path = channel_folder_path + "/" + image_name + ".jpg";
                    saveDummyJpg(image_path);

                    //ファイルパスからjpg読み込み
                    //Sprite sprite = SpriteFromFile(image_path);
                    Texture2D sprite = Texture2DFromFile(image_path);
                    sprite.name = image_name + ".jpg";

                    images.Add(image_name, sprite);
                }

            }

        }

        if (!spriteListCache.ContainsKey(channel_id))
            spriteListCache.Add(channel_id, images);

        //return images;
    }

    /// <summary>
    /// 画像が無い時用のダミーテクスチャを生成
    /// </summary>
    private void generateDummyImageByte()
    {
        m_texture = new Texture2D(512, 512, TextureFormat.ARGB32, false);

        Color col = new Color(1.0f, 0.8f, 0.8f, 1.0f);
        for (int y = 0; y < m_texture.height; y++)
        {
            for (int x = 0; x < m_texture.width; x++)
            {
                m_texture.SetPixel(x, y, col);
            }
        }
        m_texture.Apply();

        texture_bytes = m_texture.EncodeToJPG();
    }

    /// <summary>
    /// ダミー画像を生成する
    /// </summary>
    /// <param name="image_path"></param>
    private void saveDummyJpg(string image_path)
    { 
        File.WriteAllBytes(image_path, texture_bytes);
    }



    /*
    /// <summary>
    /// ローカルフォルダから画像を取得
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Dictionary<string, Texture2D> SpriteListFromFolder(string path)
    {
        if (Directory.Exists(path))
        {
            //Dictionary<string, Sprite> images = new Dictionary<string, Sprite>();
            Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();

            //フォルダ内のpngファイルのファイル名取得
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] info = dir.GetFiles("*.png");
            foreach (FileInfo file in info)
            {
                string filePath = path + "/" + file.Name;

                //ファイルパスからpng読み込み
                //Sprite sprite = SpriteFromFile(filePath);
                Texture2D sprite = Texture2DFromFile(filePath);
                sprite.name = file.Name;

                //Debug.Log(sprite.name);

                if (sprite)
                {
                    sprite.name = file.Name;
                    images.Add(file.Name, sprite);
                }
                sprite = null;
            }
            info = null;
            dir = null;

            return images;
        }
        else
        {
            return null;
        }
    }
    */

    public IEnumerator Texture2DListFromFolder(string path, Dictionary<string, Texture2D> images)
    {
        if (Directory.Exists(path))
        {

            //フォルダ内のpngファイルのファイル名取得
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] info = dir.GetFiles("*.jpg");
            foreach (FileInfo file in info)
            {

                string tex_name = file.Name.Replace(".jpg", "");

                if (!images.ContainsKey(tex_name))
                {
                    string filePath = path + "/" + file.Name;

                    //ファイルパスからjpg読み込み
                    Texture2D sprite = Texture2DFromFile(filePath);
                    sprite.name = file.Name;

                    //Debug.Log(sprite.name);

                    if (sprite)
                    {
                        sprite.name = tex_name;
                        images.Add(tex_name, sprite);
                    
                        //Debug.Log(sprite.name + " loaded");
                    }
                    sprite = null;

                    yield return 0;
                }
            }
            info = null;
            dir = null;

            Debug.Log(path + "の読み込みが完了しました。");
        }
        else
        {
            //return null;
            yield break;
        }
    }
    
    /*
    private Sprite SpriteFromFile(string path)
    {
        Sprite sprite = null;
        Texture2D texture = Texture2DFromFile(path);
        if (texture)
        {
            //Texture2DからSprite作成
            sprite = SpriteFromTexture2D(texture);
        }
        texture = null;
        return sprite;
    }
    */

    /*
    private Sprite SpriteFromTexture2D(Texture2D texture)
    {
        Sprite sprite = null;
        if (texture)
        {
            //Texture2DからSprite作成
            sprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        return sprite;
    }
    */

    public Texture2D Texture2DFromFile(string path)
    {
        Texture2D texture = null;

        if (File.Exists(path))
        {
            //Debug.Log(path + " is exist");
            
            //byte取得
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader bin = new BinaryReader(fileStream);
            byte[] readBinary = bin.ReadBytes((int)bin.BaseStream.Length);
            bin.Close();
            fileStream.Dispose();
            fileStream = null;
            if (readBinary != null)
            {
                //横サイズ
                //int pos = 16;

                int width = 512;
                int height = 512;
                /*
                int width = 0;
                for (int i = 0; i < 4; i++)
                {
                    width = width * 256 + readBinary[pos++];
                }
                //縦サイズ
                int height = 0;
                for (int i = 0; i < 4; i++)
                {
                    height = height * 256 + readBinary[pos++];
                }*/




                //byteからTexture2D作成
                texture = new Texture2D(width, height);
 
                texture.LoadImage(readBinary);
            }
            readBinary = null;
        }
        return texture;
    }
}