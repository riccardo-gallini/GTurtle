using IronPython.Compiler.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GScripting
{
    public class ScriptAstWalker : PythonWalker
    {
        private ParseInfo info;
        private IDictionary<int,Node> _codeMap;

        public ScriptAstWalker(ParseInfo parse_info)
        {
            info = parse_info;
            _codeMap = info.codeMap;
        }

        public void map(Node n)
        {
            var _start = n.Span.Start.Index;
            var _end = n.Span.End.Index;

            for (var i = _start; i <= _end; i++)
            {
                updateMap(i, n);
            }
        }

        private void updateMap(int index, Node n)
        {
            if (!_codeMap.ContainsKey(index))
            {
                _codeMap.Add(index, n);
            }
            else
            {
                _codeMap[index] = n;
            }
        }

        #region "UH"
        
        public override bool Walk(ConstantExpression node) { map(node); return base.Walk(node); }
        public override bool Walk(MemberExpression node) { map(node); return base.Walk(node); }
        public override bool Walk(NameExpression node) { map(node); return base.Walk(node); }
        public override bool Walk(Parameter node) { map(node); return base.Walk(node); }

        //public override bool Walk(FunctionDefinition node) { map(node); return base.Walk(node); }
        //public override bool Walk(ClassDefinition node) { map(node); return base.Walk(node); }
        //public override bool Walk(SetComprehension node) { map(node); return base.Walk(node); }
        //public override bool Walk(SetExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(WithStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(SliceExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(TupleExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(WhileStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(UnaryExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(YieldExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(TryStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(AssertStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(AssignmentStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(SuiteStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(AugmentedAssignStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(BreakStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(Arg node) { map(node); return base.Walk(node); }
        //public override bool Walk(ReturnStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(TryStatementHandler node) { map(node); return base.Walk(node); }
        //public override bool Walk(ContinueStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(DelStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(RaiseStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(EmptyStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(ExecStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(PythonAst node) { map(node); return base.Walk(node); }
        //public override bool Walk(ExpressionStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(ForStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(PrintStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(FromImportStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(ImportStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(GlobalStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(ParenthesisExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(IfStatement node) { map(node); return base.Walk(node); }
        //public override bool Walk(ErrorExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(ConditionalExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(DictionaryComprehension node) { map(node); return base.Walk(node); }
        //public override bool Walk(RelativeModuleName node) { map(node); return base.Walk(node); }
        //public override bool Walk(DictionaryExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(ModuleName node) { map(node); return base.Walk(node); }
        //public override bool Walk(OrExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(CallExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(GeneratorExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(IfStatementTest node) { map(node); return base.Walk(node); }
        //public override bool Walk(IndexExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(BinaryExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(SublistParameter node) { map(node); return base.Walk(node); }
        //public override bool Walk(DottedName node) { map(node); return base.Walk(node); }
        //public override bool Walk(ListComprehension node) { map(node); return base.Walk(node); }
        //public override bool Walk(BackQuoteExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(ListExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(ComprehensionIf node) { map(node); return base.Walk(node); }
        //public override bool Walk(AndExpression node) { map(node); return base.Walk(node); }
        //public override bool Walk(ComprehensionFor node) { map(node); return base.Walk(node); }
        //public override bool Walk(LambdaExpression node) { map(node); return base.Walk(node); }


        #endregion




    }
}
