using System;
using System.Collections.Generic;
using NUnit.Framework;
using static System.Math;
using static System.IO.FileOptions;

namespace CS6Trial
{
    [TestFixture]
    public class CS6Tests
    {
        public sealed class Person
        {
            public Person(string name, DateTime dateOfBirth)
            {
                DateOfBirth = dateOfBirth;
                Name = name;
            }

            public Person(string name)
            {
                Name = name;
                OnCreate();
            }

            // Read-only auto prop
            // Does not generate a private setter
            // Can only be set in ctor
            public DateTime DateOfBirth { get; } = DateTime.Now;
            public string Name { get; }

            // Expression body members allow in line
            // Uses string interpolation
            public override string ToString() => $"{Name} says hi!";

            // Expression body members are good for operators
            public static Person operator +(Person lhs, Person rhs) => Add(lhs, rhs);

            public static Person Add(Person lhs, Person rhs)
            {
                if (rhs == null)
                {
                    // Using nameof, I don't have to specify the parameter name
                    // Ensures this code is refactor friendly
                    throw new ArgumentNullException(nameof(rhs));
                }
                return new Person($"{lhs.Name} and {rhs.Name}");
            }

            public event EventHandler Create;

            private void OnCreate()
            {
                // Without this, if no one is subscribed, would throw exception
                Create?.Invoke(this, EventArgs.Empty);
            }
        }

        public class PersonDictionary
        {
            public PersonDictionary()
            {
                // Using Index initializers
                Dictionary = new Dictionary<string, Person>
                {
                    ["test"] = new Person("Me")
                };

                // Using Add extension method
                Dictionary = new Dictionary<string, Person>
                {
                    {"test", DateTime.Now, "Me"}
                };
            }

            public IDictionary<string, Person> Dictionary { get; set; }
        }

        [Test]
        public void EnumWithStaticImport()
        {
            var option = Asynchronous;
            switch (option)
            {
                case Asynchronous:
                    Assert.Pass();
                    break;
                case DeleteOnClose:
                case Encrypted:
                case None:
                case RandomAccess:
                case SequentialScan:
                case WriteThrough:
                    Assert.Fail();
                    break;
            }
        }

        [Test]
        public void ExceptionFilters()
        {
            try
            {
                Person.Add(new Person("Me"), null);
            }
            catch (ArgumentNullException ex) when (ex.ParamName == "rhs")
            {
                Assert.Pass();
            }
            catch (ArgumentNullException ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void PersonAddException()
        {
            var parameterName = typeof (Person).GetMethod(nameof(Person.Add)).GetParameters()[1].Name;
            Assert.Throws<ArgumentNullException>(() => Person.Add(new Person("Me"), null), parameterName);
        }

        [Test]
        public void PersonInitializerWithStaticImport()
        {
            // Static imports can be brought in one file at a time
            // This allows me to extend something basic like an integer
            // And not bleed that extension into every other piece of code using ints
            var myBirthday = 1.May(1986);
            var p = new Person("Me", myBirthday);
            Assert.That(p.DateOfBirth, Is.EqualTo(myBirthday));
        }

        [Test]
        public void PersonToString()
        {
            Assert.That(new Person("Bob").ToString(), Is.EqualTo("Bob says hi!"));
        }

        [Test]
        public void StaticImportMath()
        {
            Assert.That(PI, Is.GreaterThanOrEqualTo(3.14).And.LessThanOrEqualTo(3.15));
        }
    }

    public static class CS6Extensions
    {
        public static DateTime May(this int day, int year)
        {
            return new DateTime(year, 5, day);
        }

        public static void Add(this IDictionary<string, CS6Tests.Person> d, string key, DateTime dob, string name)
        {
            d.Add(key, new CS6Tests.Person(name));
        }
    }
}