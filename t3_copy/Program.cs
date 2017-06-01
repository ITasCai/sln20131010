using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace t3_copy
{
    class Program
    {
        static void Main(string[] args)
        {
            Person p1 = new Person() { Name = "sk", Gender = "male", Age = 18 };
            //Person p2 = p1;//没有发生拷贝，只是变量指向了一个对象的引用

            //拷贝
            //Person p2 = new Person();
            //p2.Name = p1.Name;
            //p2.Gender = p1.Gender;
            //p2.Age = p1.Age;

            //序列化完成拷贝
            //Person p2;
            //BinaryFormatter bf = new BinaryFormatter();
            //byte[] bs = new byte[1000];
            //using (MemoryStream ms = new MemoryStream(bs))
            //{
            //    bf.Serialize(ms, p1);
            //}
            //using (MemoryStream ms = new MemoryStream(bs))
            //{
            //    p2 = bf.Deserialize(ms) as Person;
            //}

            //浅拷贝
            //p1.MyCar = new Car() { Brand = "audi" };
            //Person p2 = new Person();
            //p2.Name = p1.Name;
            //p2.Gender = p1.Gender;
            //p2.Age = p1.Age;
            //p2.MyCar = p1.MyCar;//这段代码表示是浅拷贝

            //深拷贝
            //p1.MyCar = new Car() { Brand = "audi" };
            //Person p2 = new Person();
            //p2.Name = p1.Name;
            //p2.Age = p1.Age;
            //p2.Gender = p1.Gender;
            //p2.MyCar = new Car();//这段代码是深拷贝
            //p2.MyCar.Brand = p1.MyCar.Brand;

            //通过序列化完成深拷贝（所有的序列化操作都是进行的深拷贝）
            p1.MyCar = new Car() { Brand = "qq" };
            BinaryFormatter bf = new BinaryFormatter();
            byte[] bs = new byte[2000];
            using (MemoryStream ms = new MemoryStream(bs))
            {
                bf.Serialize(ms, p1);
            }
            Person p2;
            using (MemoryStream ms = new MemoryStream(bs))
            {
                p2 = bf.Deserialize(ms) as Person;
            }

            Console.ReadKey();
        }
    }
    [Serializable]
    class Person
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string gender;


        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }
        private int age;

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        private Car myCar;

        internal Car MyCar
        {
            get { return myCar; }
            set { myCar = value; }
        }
    }
    [Serializable]
    class Car
    {
        private string brand;


        public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }
    }
}
