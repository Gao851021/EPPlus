/*************************************************************************************************
  Required Notice: Copyright (C) EPPlus Software AB. 
  This software is licensed under PolyForm Noncommercial License 1.0.0 
  and may only be used for noncommercial purposes 
  https://polyformproject.org/licenses/noncommercial/1.0.0/

  A commercial license to use this software can be purchased at https://epplussoftware.com
 *************************************************************************************************
  Date               Author                       Change
 *************************************************************************************************
  01/27/2020         EPPlus Software AB       Initial release EPPlus 5
 *************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;

namespace OfficeOpenXml.FormulaParsing.Excel.Functions.Math
{
    internal class SumIfs : MultipleRangeCriteriasFunction
    {
        public override CompileResult Execute(IEnumerable<FunctionArgument> arguments, ParsingContext context)
        {
            var functionArguments = arguments as FunctionArgument[] ?? arguments.ToArray();
            ValidateArguments(functionArguments, 3);
            var rows = new List<int>();
            var valueRange = functionArguments[0].ValueAsRangeInfo;
            List<double> sumRange;
            if(valueRange != null)
            {
                sumRange = ArgsToDoubleEnumerableZeroPadded(true, valueRange, context).ToList();
            }
            else
            {
                sumRange = ArgsToDoubleEnumerable(true, new List<FunctionArgument> { functionArguments[0] }, context).Select(x => (double)x).ToList();
            } 
            var argRanges = new List<ExcelDataProvider.IRangeInfo>();
            var criterias = new List<string>();
            for (var ix = 1; ix < 31; ix += 2)
            {
                if (functionArguments.Length <= ix) break;
                var rangeInfo = functionArguments[ix].ValueAsRangeInfo;
                argRanges.Add(rangeInfo);
                var value = functionArguments[ix + 1].Value != null ? ArgToString(arguments, ix + 1) : null;
                criterias.Add(value);
            }
            IEnumerable<int> matchIndexes = GetMatchIndexes(argRanges[0], criterias[0]);
            var enumerable = matchIndexes as IList<int> ?? matchIndexes.ToList();
            for (var ix = 1; ix < argRanges.Count && enumerable.Any(); ix++)
            {
                var indexes = GetMatchIndexes(argRanges[ix], criterias[ix]);
                matchIndexes = enumerable.Intersect(indexes);
            }

            var result = matchIndexes.Sum(index => sumRange[index]);

            return CreateResult(result, DataType.Decimal);
        }
    }
}