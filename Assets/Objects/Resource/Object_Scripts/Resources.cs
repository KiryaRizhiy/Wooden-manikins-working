using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public static class Resources{
    
    public static List<Resource> ResourceLibrary { get; private set; }

    private static List<Deposit> ResourcesDeposits;
    private static XmlDocument _ResourceConfig = new XmlDocument();
    private static string _TextureExtention = ".png";
    private static string scr = "Resources", scrd = "ResourcesDetails";

	// Подгружаем в оперативку конфиг ресурсов
    public static void UploadResources()
    {
        ResourcesDeposits = new List<Deposit>();
        Resource _r;
        ResourceLibrary = new List<Resource>();
        _ResourceConfig.Load("C:/GD/You and world/v.0.0.1/First/Configs/objects/Resource.xml");
        foreach (XmlNode _res in _ResourceConfig.DocumentElement.ChildNodes)
        {
            //Validation
            if (!(Functions.IsNodeNotEmpty(_res.ChildNodes.Item(0), scrd) && Functions.IsNodeNotEmpty(_res.ChildNodes.Item(1), scrd) && Functions.IsNodeNotEmpty(_res.ChildNodes.Item(2), scrd)))
            {
                Log.Notice(scr,"Resource is invalid. Type = " + _res.ChildNodes.Item(0).InnerText + ", Name = " + _res.ChildNodes.Item(1).InnerText + ", Material = " + _res.ChildNodes.Item(2).InnerText);
                continue;
            }
            //Uploading

            ResourceLibrary.Add(new Resource());
            _r = ResourceLibrary.FindLast(x => true);
            _r.Type = ushort.Parse(_res.ChildNodes.Item(0).InnerText);
            _r.Name = _res.ChildNodes.Item(1).InnerText;
            //Debug.Log("Loading " + _r.Name);
            _r.Material = CreateMaterialWithTexture(Settings.TexturesFolderPath + _res.ChildNodes.Item(2).InnerText + _TextureExtention);
            _r.Sprite = CreateSpriteWithTexture(Settings.TexturesFolderPath + _res.ChildNodes.Item(2).InnerText + _TextureExtention);
            if (Functions.IsNodeNotEmpty(_res.ChildNodes.Item(3), scrd))
            {
                List<ResourceDetail> _Details = new List<ResourceDetail>();
                foreach (XmlNode _d in _res.ChildNodes.Item(3).ChildNodes)
                    _Details.Add(new ResourceDetail(ushort.Parse(_d.ChildNodes.Item(0).InnerText), byte.Parse(_d.ChildNodes.Item(1).InnerText)));
                _r.Details = _Details;
            }
            if (Functions.IsNodeNotEmpty(_res.ChildNodes.Item(4), scrd))
                _r.MachineToCreate = ushort.Parse(_res.ChildNodes.Item(4).InnerText);
            if (Functions.IsNodeNotEmpty(_res.ChildNodes.Item(5), scrd))
                _r.TimeToCreate = float.Parse(_res.ChildNodes.Item(5).InnerText);
            if (Functions.IsNodeNotEmpty(_res.ChildNodes.Item(6), scrd))
                _r.TimeToMine = float.Parse(_res.ChildNodes.Item(6).InnerText);
            if (Functions.IsNodeNotEmpty(_res.ChildNodes.Item(7), scrd))
                _r.TimeToBuild = float.Parse(_res.ChildNodes.Item(7).InnerText);
            if (Functions.IsNodeNotEmpty(_res.ChildNodes.Item(8), scrd))
                _r.Volume = byte.Parse(_res.ChildNodes.Item(8).InnerText);
            if (byte.Parse(_res.ChildNodes[9].InnerText) == 1)
            {
                _r.IsResourceForWorldBuilding = true;
                _r.UpperBoarderPercent = ushort.Parse(_res.ChildNodes[10].InnerText);
                _r.LowerBoarderPercent = ushort.Parse(_res.ChildNodes[11].InnerText);
            }
            else
                _r.IsResourceForWorldBuilding = false;
            _r.Figure = (DepositFigure)int.Parse(_res.ChildNodes[12].InnerText);
            _r.MayBeUsedInStructures = byte.Parse(_res.ChildNodes[13].InnerText) == 1;
            _r.Classification = (ResourceTypeClassification)int.Parse(_res.ChildNodes[14].InnerText);
            //_r.Log();
            //Debug.Log(_r.Name + " loaded to library");
        }
        //Debug.Log("Library of resiurces constructed. It contains " + ResourceLibrary.Count + " elements");
        //foreach (Resource _res in ResourceLibrary)
        //{
        //    Debug.Log(_res.Type + " : " + _res.Name);
        //}
    }
    private static Material CreateMaterialWithTexture(string Path)
    {
        if (File.Exists(Path))
        {
            //Texture2D _Tx = new Texture2D(16, 16);
            //_Tx.LoadImage((byte[])_Converter.ConvertTo(Image.FromFile(Path), typeof(byte[])));
            //Material _M = new Material(MaterialPrefab);
            //_M.mainTexture = _Tx;
            //return _M;

            // alternative solution. Smart people recomends not to use System.Drawing namespace.
            FileInfo _img = new FileInfo(Path);
            MemoryStream _dest = new MemoryStream();

            using (Stream source = _img.OpenRead())
            {
                byte[] _buffer = new byte[1024];
                int _bytesread;
                while ((_bytesread = source.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    _dest.Write(_buffer, 0, _bytesread);
                }
            }

            byte[] _imgbytes = _dest.ToArray();
            Texture2D _Tx = new Texture2D(16, 16);

            _Tx.LoadImage(_imgbytes);

            Material _M = new Material(Settings.ResourceBasicMaterial);
            _M.mainTexture = _Tx;
            return _M;
        }
        else
        {
            Log.Warning(scr,"File " + Path + " does not exitst");
            return Settings.ResourceBasicMaterial;
        }
    }
    private static Sprite CreateSpriteWithTexture(string Path)
    {
        if (File.Exists(Path))
        {
            //Texture2D _Tx = new Texture2D(16, 16);
            //_Tx.LoadImage((byte[])_Converter.ConvertTo(Image.FromFile(Path), typeof(byte[])));
            //Material _M = new Material(MaterialPrefab);
            //_M.mainTexture = _Tx;
            //return _M;

            // alternative solution. Smart people recomends not to use System.Drawing namespace.
            FileInfo _img = new FileInfo(Path);
            MemoryStream _dest = new MemoryStream();

            using (Stream source = _img.OpenRead())
            {
                byte[] _buffer = new byte[1024];
                int _bytesread;
                while ((_bytesread = source.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    _dest.Write(_buffer, 0, _bytesread);
                }
            }

            byte[] _imgbytes = _dest.ToArray();
            Texture2D _Tx = new Texture2D(16, 16);

            _Tx.LoadImage(_imgbytes);
            return Sprite.Create(_Tx, new Rect(0,0,16,16),Vector2.zero);
        }
        else
        {
            Log.Warning(scr,"File " + Path + " does not exitst");
            return Sprite.Create(new Texture2D(16, 16), new Rect(0, 0, 16, 16), Vector2.zero);
        }
    }
    public static Resource GetResource(ushort Type)
    {
        foreach (Resource _res in ResourceLibrary)
        {
            if (_res.Type == Type)
            {
                Log.Notice(scr,_res.Type + " : " + _res.Name);
                return _res;
            }
        }
        Log.Warning(scr,"No resource of type " + Type + " found");
        return null;
        //if (ResourceLibrary.Find(x => x.Type == Type) == null) To figure out why the fuck it doesn't works
        //{
        //    Debug.LogError("No resource of type " + Type + " found");
        //    Debug.Log(ResourceLibrary.Find(x => x.Type == Type));
        //    return null;
        //}
        //else
        //    return ResourceLibrary.Find(x => x.Type == Type);
    }

    //Генерация залежей
    /// <summary>
    /// Получение ресурса, который должен лежать в даннной точке
    /// </summary>
    /// <param name="Coordinates"></param>
    /// <returns></returns>
    public static Resource GetResourceInPoint(Vector3 Coordinates)
    {
        //Времянка. Норм логика ниже
        //if (Settings.MapSoilLevel - (Settings.MapHeight - Coordinates.y) + Random.Range(-2, 2) > 0) //Если до уровня почвы еще не добрались
        //    return GetResource(2);//Создаем камень
        //else//Иначе
        //    return GetResource(1);//Создаем землю
        if (ResourcesDeposits.Count > 0)
        {
            Deposit _d = ResourcesDeposits.Find(x => x.IsBrickInDeposit(Coordinates));
            if (_d != null)
                return _d.DepositResource;
        }
        return GetResource(2);//Возвращаем землю по дефолту
    }
    public static void ExtendDepositsGrid(Vector2 Coordinates)
    {
        Log.Notice(scr, "Extending deposits grid to kernel " + Coordinates);
        //Логика расширения сетки залежей
        //Берем из сеттингов параметр частоты распределения точек ресурсов. Считаем по нему сколько залежей добавим в стержень
        for (int i = 0; i < Settings.ResourceGeneration.DepositsQuantityParameter; i++)
        {
            Resource _DepositResource = GetResourceForDeposit();//Находим рандомный ресурс для заложения
            Vector2 _Position = Vector2.up * Random.value * Settings.KernelSize + Vector2.right * Random.value * Settings.KernelSize; //определяем ему рандомные V2 координаты 
            float _depth = Settings.MapHeight * (1 - Random.Range(_DepositResource.UpperBoarderPercent, _DepositResource.LowerBoarderPercent) / 100f);//и по его настройкам вычисляем глубину заложения.
            //Log.Notice(scr,_depth + " " + _DepositResource.UpperBoarderPercent + " " + _DepositResource.LowerBoarderPercent + " " + Settings.MapHeight + " " + Random.Range(_DepositResource.UpperBoarderPercent, _DepositResource.LowerBoarderPercent));
            Vector3 _DepositCenter = new Vector3(_Position.x + Coordinates.x * Settings.KernelSize, _depth, _Position.y + Coordinates.y * Settings.KernelSize);
            Log.Notice(scr, "Add new deposit of resource " + _DepositResource.Name + " at " + _DepositCenter);
            ResourcesDeposits.Add(new Deposit(_DepositCenter, _DepositResource));
        }
        //Генерим залежи
    }
    /// <summary>
    /// Выдает рандомный ресурс для закладывания в залежи
    /// </summary>
    /// <returns></returns>
    private static Resource GetResourceForDeposit()
    {
        return Resources.ResourceLibrary.FindAll(x => x.IsResourceForWorldBuilding)[Random.Range(0, Resources.ResourceLibrary.FindAll(x => x.IsResourceForWorldBuilding).Count)];
    }
}