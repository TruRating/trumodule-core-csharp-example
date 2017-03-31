// The MIT License
// 
// Copyright (c) 2017 TruRating Ltd. https://www.trurating.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TruRating.TruModule.V2xx.Tests.Unit
{
    public abstract class MsTestsContext
    {
        [TestInitialize]
        public void TestInitialize()
        {
            InitContainer();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DisposeMockContainer();
        }

        protected static RhinoMockingContainer Container;

        protected static void InitContainer()
        {
            Container = new RhinoMockingContainer();
        }

        protected static TMock MockOf<TMock>()
        {
            return Container.MockOf<TMock>();
        }

        protected static TMock CreateWithMocks<TMock>() where TMock : class
        {
            return Container.Create<TMock>();
        }

        protected static void RegisterFake<TMock>(TMock instance) where TMock : class
        {
            Container.Register(instance);
        }

        protected static void DisposeMockContainer()
        {
            Container.Dispose();
            Container = null;
        }
    }
    public abstract class MsTestsContext<T> where T : class
    {
        private static T _sut;

        protected static T Sut
        {
            get { return _sut ?? (_sut = CreateWithMocks<T>()); }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            InitContainer();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _sut = null;
            DisposeMockContainer();
        }

        protected static RhinoMockingContainer Container;

        protected static void InitContainer()
        {
            Container = new RhinoMockingContainer();
        }

        protected static TMock MockOf<TMock>()
        {
            return Container.MockOf<TMock>();
        }

        protected static TMock CreateWithMocks<TMock>() where TMock : class
        {
            return Container.Create<TMock>();
        }

        protected static void RegisterFake<TMock>(TMock instance) where TMock : class
        {
            Container.Register(instance);
        }

        protected static void DisposeMockContainer()
        {
            Container.Dispose();
            Container = null;
        }
    }
}