using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.DLinq.Extensions;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq;

public class ExpressionEngine
{
    internal static bool IsSimple(string filter)
    {
        // simple filter might contain property or comma-separated properties
        // any operator like indexer [], dot `.`, or comparison operators is a complex expression that needs to be parsed with engine
        return !filter.ContainsAny('[', '.', '@', '=', '<', '>');
    }


    private readonly LexemeParser _parser = new();

    /// <summary>
    /// Projects each element of sequence into a new form
    /// (dynamic select to list operation i.e. source.Select(expression).ToList())
    /// </summary>
    public IEnumerable Process<T>(IEnumerable<T> source, string? filterExpression, Type? targetType)
    {
        var lexemes = _parser.ParseExpression(filterExpression);
        if (lexemes.Length == 0)
            return source;

        var typeArg = targetType ?? typeof(T);

        if (lexemes.Length == 1)
        {
            return ProcessLexeme(source, lexemes[0], typeArg);
        }

        if (lexemes.All(x => x.IsSimple))
        {
            var props = typeArg.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase, lexemes.Select(x => x.Property));
            return source.ToList(props, typeArg);
        }

        return ProcessLexemes(source, lexemes, typeArg);
    }

    private IEnumerable ProcessLexemes<T>(IEnumerable<T> source, Lexeme[] lexemes, Type typeArg)
    {
        var propsDict = typeArg.SearchProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase, lexemes.Select(x => x.Property).Distinct());

        var commonResultType = true;
        Type? resultType = null;

        var spec = new ListDictLambdaSpec<T>(lexemes.Length, typeArg);

        foreach (var lexeme in lexemes)
        {
            if(!propsDict.ContainsKey(lexeme.Property))
                continue;

            var prop = propsDict[lexeme.Property];
            spec.AddItem(lexeme, prop);

            var type = lexeme.GetResultType(prop);
            resultType ??= type;

            if(commonResultType && type != resultType)
                commonResultType = false;
        }

        if (resultType == null)
            return source;

        if (commonResultType)
        {
            spec.ValueType = resultType;
            return ToListOfTypedDictionary(source, spec);
        }

        return ToListOfObjectDictionary(source, spec);
    }

    private IEnumerable ToListOfObjectDictionary<T>(IEnumerable<T> source, ListDictLambdaSpec<T> spec)
    {
        // select list of object dictionary
        var fn = LambdaBag.Lambdas.GetSelectListOfObjectDictFn(spec);
        return fn.Invoke(source);
    }

    private IEnumerable ToListOfTypedDictionary<T>(IEnumerable<T> source, ListDictLambdaSpec<T> spec)
    {
        // select list of typed dictionary
        var fn = LambdaBag.Lambdas.GetSelectListOfTypedDictFn(spec);
        return fn.Invoke(source);
    }


    private IEnumerable ProcessLexeme<T>(IEnumerable<T> source, Lexeme lexeme, Type targetType)
    {
        var prop = targetType.GetProperty(lexeme.Property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null)
            return source;

        Func<IEnumerable<T>, IEnumerable> selectFn;

        switch (lexeme.GetExpressionType())
        {
            case ExpressionType.Compound:
                throw new NotImplementedException("Compound expressions (inner props) not supported yet");

            case ExpressionType.Index:
            {
                selectFn = LambdaBag.Lambdas.GetSelectListByIndexFn<T>(prop, lexeme.Index, targetType);
                break;
            }
            case ExpressionType.Key:
            {
                selectFn = LambdaBag.Lambdas.GetSelectListByKeyFn<T>(prop, lexeme.Key!, targetType);
                break;
            }
            case ExpressionType.Default:
            default:
            {
                selectFn = LambdaBag.Lambdas.GetSelectListFn<T>(prop, targetType);
                break;
            }
        }

        return selectFn.Invoke(source);
    }
}