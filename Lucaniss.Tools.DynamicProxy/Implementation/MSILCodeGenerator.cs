using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Lucaniss.Tools.DynamicProxy.Extensions;
using Lucaniss.Tools.DynamicProxy.Implementation.Interceptors.Implementations;


namespace Lucaniss.Tools.DynamicProxy.Implementation
{
    internal static class MSILCodeGenerator
    {
        public static LocalBuilder DeclareLocalVariable(ILGenerator msil, Type type)
        {
            // INFO: Zadeklarowanie zmiennej lokalnej o podanym typie.
            return msil.DeclareLocal(type);
        }

        public static void CreateConstructor(ILGenerator msil, MSILCodeVariables variables)
        {
            // INFO: Odłożenie na stosie obiektu 'this'.
            msil.Emit(OpCodes.Ldarg_0);

            // INFO: Odłożenie na stosie obiektu przekazanego w parametrze konstruktora (Oryginał).
            msil.Emit(OpCodes.Ldarg_1);

            // INFO: Skopiowanie obiektu Interceptora do lokalnego pola klasy Proxy.
            msil.Emit(OpCodes.Stfld, variables.OriginalInstanceFieldInfo);


            // INFO: Odłożenie na stosie obiektu 'this'.
            msil.Emit(OpCodes.Ldarg_0);

            // INFO: Odłoznie na stosie obiektu przekazanego w parametrze konstruktora (Interceptor)
            msil.Emit(OpCodes.Ldarg_2);

            // INFO: Skopiowanie obiektu Interceptora do lokalnego pola instancji klasy Proxy.
            msil.Emit(OpCodes.Stfld, variables.InterceptorInstanceFieldInfo);


            // INFO: Odłożenie na stosie obiektu 'this'.
            msil.Emit(OpCodes.Ldarg_0);

            // INFO: Odłożenie na stosie obiektu przekazanego w parametrze konstruktora (Interceptor).
            msil.Emit(OpCodes.Ldarg_3);

            // INFO: Skopiowanie obiektu Handlera do lokalnego pola instancji klasy Proxy.
            msil.Emit(OpCodes.Stfld, variables.InterceptorHandlerInstanceFieldInfo);


            // INFO: Powrót z podprogramu.
            msil.Emit(OpCodes.Ret);
        }

        public static void CreateArrayForArgumentTypes(ILGenerator msil, MethodInfo methodInfo, LocalBuilder arrayVariable)
        {
            // INFO: Stworzenie tablicy przechowującej nazwy typów argumentów metody oraz przypisanie jej do zmiennej lokalnej.
            //       1. Odłożenie na stosie liczby określającej rozmiar tablicy.
            //       2. Utworzenie na stosie tablicy o rozmiarze wskazanym przez wartość na stosie.
            msil.Emit(OpCodes.Ldc_I4, methodInfo.GetParameters().Count());
            msil.Emit(OpCodes.Newarr, typeof (String));

            // INFO: Zdjęcie ze stosu nowo utworzonej tablicy i przypisanie jej do zmiennej lokalnej.
            msil.Emit(OpCodes.Stloc, arrayVariable);
        }

        public static void CraeteArrayForArgumentValues(ILGenerator msil, MethodInfo methodInfo, LocalBuilder arrayVariable)
        {
            // INFO: Stworzenie tablicy przechowującej wartości argumentów metody oraz przypisanie jej do zmiennej lokalnej.
            //       1. Odłożenie na stosie liczby określającej rozmiar tablicy.
            //       2. Utworzenie na stosie tablicy o rozmiarze wskazanym przez wartość na stosie.
            msil.Emit(OpCodes.Ldc_I4, methodInfo.GetParameters().Count());
            msil.Emit(OpCodes.Newarr, typeof (Object));

            // INFO: Zdjęcie ze stosu nowo utworzonej tablicy i przypisanie jej do zmiennej lokalnej.
            msil.Emit(OpCodes.Stloc, arrayVariable);
        }

        public static void StoreArgumentTypeNameInArray(ILGenerator msil, ParameterInfo parameter, Int32 index, LocalBuilder arrayVariable)
        {
            // INFO: Odłożenie na stosie elementów potrzebnych do skopiowania wartości parametru metody do tablicy obiektów.
            msil.Emit(OpCodes.Ldloc, arrayVariable); // Odłożenie na stosie zmiennej lokalnej reprezentującej tablicę obiektów.
            msil.Emit(OpCodes.Ldc_I4, index); // Odłożenie na stosie liczby reprezentującej indeks elementu tablicy do którego ma zostać skopiowana wartość.
            msil.Emit(OpCodes.Ldstr, parameter.ParameterType.SafeGetTypeName()); // Odłożenie na stosie nazwy typu argumentu metody.

            // INFO: Skopiowanie nazwy typu argumentu metody do tablicy obiektów.
            msil.Emit(OpCodes.Stelem_Ref);
        }

        public static void StoreArgumentValueInArray(ILGenerator msil, ParameterInfo parameter, Int32 index, LocalBuilder arrayVariable)
        {
            // INFO: Odłożenie na stosie elementów potrzebnych do skopiowania wartości parametru metody do tablicy obiektów.
            msil.Emit(OpCodes.Ldloc, arrayVariable); // Odłożenie na stosie zmiennej lokalnej reprezentującej tablicę obiektów.
            msil.Emit(OpCodes.Ldc_I4, index); // Odłożenie na stosie liczby reprezentującej indeks elementu tablicy do którego ma zostać skopiowana wartość.          

            // INFO: Odłożenie na stosie wartości argumentu metody o podanym indeksie.
            //       W przypadku parametrów referencyjnych (ref, out) odkładany jest adres.
            msil.Emit(OpCodes.Ldarg, index + 1);

            // INFO: Jeśli parametr jest przekazywany przez referencję (ref, out).
            if (parameter.ParameterType.IsByRef)
            {
                if (parameter.IsValueOrPrimitiveType())
                {
                    // INFO: Odłożenie na stosie obiektu wskazywanego przez adres argumentu metody o podanym indeksie (dla typów wartościowych).
                    msil.Emit(OpCodes.Ldobj, parameter.SafeGetType());
                }
                else
                {
                    // INFO: Odłożenie na stosie referencji argumentu metody o podanym indeksie (dla typów referencyjnych).
                    msil.Emit(OpCodes.Ldind_Ref);
                }
            }

            // INFO: Jeśli parametr jest typem prostym wtedy wymagana jest operacja 'Box'.
            if (parameter.IsValueOrPrimitiveType())
            {
                // INFO: Wykonanie operacji 'Box' na aktualnym elemencie stosu (wartość/referencja argumentu metody).
                msil.Emit(OpCodes.Box, parameter.SafeGetType());
            }

            // INFO: Skopiowanie wartości argumentu metody do tablicy obiektów.
            msil.Emit(OpCodes.Stelem_Ref);
        }

        public static void InvokeProxyInterceptorMethod(ILGenerator msil, MethodInfo methodInfo, MSILCodeVariables variables)
        {
            // INFO: Odłożenie na stosie referencji 'this._proxyInterceptorInstance'.
            msil.Emit(OpCodes.Ldarg_0);
            msil.Emit(OpCodes.Ldfld, variables.InterceptorInstanceFieldInfo);

            // INFO: Odłożenie na stosie referencji 'this._proxyInterceptorHandlerInstance'
            msil.Emit(OpCodes.Ldarg_0);
            msil.Emit(OpCodes.Ldfld, variables.InterceptorHandlerInstanceFieldInfo);

            // INFO: Odłożenie na stosie referencji 'this._proxyOrgiginalInstance
            msil.Emit(OpCodes.Ldarg_0);
            msil.Emit(OpCodes.Ldfld, variables.OriginalInstanceFieldInfo);

            // INFO: Odłożenie na stosie nazwy metody.
            msil.Emit(OpCodes.Ldstr, methodInfo.Name);

            // INFO: Odłożenie na stosie tablicy obiektów (Nazwy typów argumentów metody).
            msil.Emit(OpCodes.Ldloc, variables.ArrayForArgumentTypesVariable);

            // INFO: Odłożenie na stosie tablicy obiektów (Wartości argumentów metody).
            msil.Emit(OpCodes.Ldloc, variables.ArrayForArgumentValuesVariable);

            // INFO: Wywołanie metody interceptora. Jeśli metoda zwraca wartość to ta wartość jest odkładana na stos.
            msil.Emit(OpCodes.Call, ProxyInterceptor.GetInterceptorMethodInfo());
        }

        public static void HandleProxyInterceptorMethodReturnValue(ILGenerator msil, MethodInfo methodInfo)
        {
            // INFO: Jeśli zwracana wartość jest typu wartościowego wtedy wymagana jest operacja 'Unbox'.
            if (methodInfo.ReturnType != typeof (void) && (methodInfo.ReturnType.IsValueOrPrimitiveType()))
            {
                // INFO: Wykonanie operacji 'Unbox' (na wskazany typ) na aktualnym elemencie stosu.
                msil.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);
            }
        }

        public static void AssignReferenceArgumentValues(ILGenerator msil, ParameterInfo[] parameters, LocalBuilder arrayForParameterValues)
        {
            for (var index = 0; index < parameters.Length; index++)
            {
                if (parameters[index].ParameterType.IsByRef)
                {
                    // INFO: Odłożenie na stosie wartości argumentu metody (w tym wypadku adres).
                    msil.Emit(OpCodes.Ldarg, index + 1);

                    // INFO: Odłożenie na stosie referencji ze zmiennej lokalnej tablicowej o podanym indeksie.
                    msil.Emit(OpCodes.Ldloc, arrayForParameterValues);
                    msil.Emit(OpCodes.Ldc_I4, index);
                    msil.Emit(OpCodes.Ldelem_Ref);

                    if (parameters[index].IsValueOrPrimitiveType())
                    {
                        // INFO: Skopiowanie obiektu o typie wartościowym pod wskazany adres argumentu o podanym indeksie (wymaga operacji 'Unbox').
                        msil.Emit(OpCodes.Unbox_Any, parameters[index].ParameterType.GetElementType());
                        msil.Emit(OpCodes.Stobj, parameters[index].ParameterType.GetElementType());
                    }
                    else
                    {
                        // INFO: Skopiowanie obiektu o typie referencyjnym pod wskazany adres argumentu o podanym indeksie.
                        msil.Emit(OpCodes.Stind_Ref);
                    }
                }
            }
        }

        public static void ReturnFromMethod(ILGenerator msil, MethodInfo methodInfo)
        {
            // INFO: Ponieważ metoda interceptora zwraca zawsze wartość (obiekt o typie referencyjnyn) 
            //       to dla metod które nie zwracają wartości (void) należy zdjąć wartość ze stosu.
            if (methodInfo.ReturnType == typeof (void))
            {
                msil.Emit(OpCodes.Pop);
            }

            // INFO: Powrót z metody.
            msil.Emit(OpCodes.Ret);
        }
    }
}