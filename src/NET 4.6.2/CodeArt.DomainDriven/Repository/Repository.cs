using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;

using CodeArt;
using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Runtime;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    public static class Repository
    {
        /// <summary>
        /// 注册仓储，请确保<paramref name="repository"/>是线程访问安全的
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="repository"></param>
        public static void Register<TRepository>(IRepository repository) where TRepository : IRepository
        {
            RepositoryFactory.Register<TRepository>(repository);
        }

        /// <summary>
        /// 创建一个仓储对象，同一类型的仓储对象会被缓存
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        public static TRepository Create<TRepository>() where TRepository : class, IRepository
        {
            return RepositoryFactory.Create<TRepository>();
        }

        /// <summary>
        /// 根据对象类型得到对象使用的仓储对象
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static IRepository Create(Type objectType)
        {
            var objectTip = ObjectRepositoryAttribute.GetTip(objectType, true);
            return RepositoryFactory.Create(objectTip.RepositoryInterfaceType);
        }


        /// <summary>
        /// 得到对象类型对应的仓储上定义的方法
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        internal static MethodInfo GetMethodFromRepository(Type objectType, string methodName)
        {
            if (string.IsNullOrEmpty(methodName)) return null;
            var objectTip = ObjectRepositoryAttribute.GetTip(objectType, true);
            var repositoryType = RepositoryFactory.GetRepositoryType(objectTip.RepositoryInterfaceType);
            var method = repositoryType.ResolveMethod(methodName);
            if (method == null)
                throw new DomainDrivenException(string.Format(Strings.NoDefineMethodFromRepository, repositoryType.FullName, methodName));
            return method;
        }


        #region 远程对象

        /// <summary>
        /// 查找远程根对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static dynamic FindRemoteRoot<T>(object id)
            where T : AggregateRootDefine
        {
            var define = TypeDefine.GetDefine<T>();
            return RemotePortal.GetObject(define, id);
        }

        /// <summary>
        /// 注册远程服务的实现
        /// </summary>
        /// <param name="implement"></param>
        public static void Register(IRemoteService implement)
        {
            RemotePortal.Register(implement);
        }

        #endregion


    }
}
