using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
//using static System.Console;

namespace R5.Internals.Abstractions.Expressions
{
	public class MisterInspector : ExpressionVisitor
	{
		private int _depth { get; set; } = 0;
		private string _indentStr { get; set; } = "    ";

		private bool _useRecursionLine { get; set; } = true;
		private string _recurseDown { get; set; } = "------------------ visit ------------------";
		private string _recurseUp { get; set; } =   "------------------ return -----------------";

		private bool _interactive { get; set; } = false; // default = false

		//private string _indent => new StringBuilder(_indentStr.Length * _depth)
		//		.Insert(0, _indentStr, _depth)
		//		.ToString();

		private void WriteLine(string s)
		{
			string indent = _depth < 1
				? ""
				: new StringBuilder(_indentStr.Length * _depth)
						.Insert(0, _indentStr, _depth)
						.ToString();

			Console.WriteLine($"{indent}{s}");
		}

		private void PauseIfInteractive()
		{
			if (_interactive)
			{
				Console.WriteLine("Press [spacebar] to continue.." + Environment.NewLine);
				while (Console.ReadKey(true).Key != ConsoleKey.Spacebar) { }
			}
		}

		private void WriteExpressionCommon(Expression node)
		{
			string expressionName = node is LambdaExpression ? "LambdaExpression" : node.GetType().Name;
			WriteLine($"[{expressionName}] {node.NodeType} ({node.Type.Name})");
		}

		private void onVisit()
		{
			PauseIfInteractive();

			_depth++;
			if (_useRecursionLine)
			{
				Console.WriteLine(_recurseDown);
			}
		}

		private void onReturn()
		{
			_depth--;
			if (_useRecursionLine)
			{
				Console.WriteLine(_recurseUp);
			}
		}

		//private string Ident()
		//{
		//	return new StringBuilder(_indentStr.Length * _depth)
		//		.Insert(0, _indentStr, _depth)
		//		.ToString();
		//}

		public void Inspect(Expression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression), "I don't know how to inspect null.");
			}

			WriteLine($"Beginning inspection of expression of NodeType '{expression.NodeType}'.");
			WriteLine($"Expression: {expression}{Environment.NewLine}");

			PauseIfInteractive();
			Visit(expression);

			WriteLine("Finished inspection. Everything looks good!");
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			WriteExpressionCommon(node);

			// todo: print details of left and right operands
			onVisit();
			Visit(node.Left);

			onVisit();
			Visit(node.Right);

			//WriteLine($"[{nameof(BinaryExpression)}] NodeType '{node.NodeType}' (Type '{node.Type.Name}')");
			//WriteLine($"Visiting '{nameof(BinaryExpression)}' with NodeType '{node.NodeType}' (Type '{node.Type.Name}')");

			onReturn();
			return node;
		}

		protected override Expression VisitBlock(BlockExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitConditional(ConditionalExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			WriteExpressionCommon(node);
			//WriteLine($"[{nameof(ConstantExpression)}] NodeType '{node.NodeType}' (Type '{node.Type.Name}')");
			WriteLine($"Value: {node.Value}");

			onReturn();
			return node;
		}

		protected override Expression VisitDebugInfo(DebugInfoExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitDefault(DefaultExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitDynamic(DynamicExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitExtension(Expression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitGoto(GotoExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitIndex(IndexExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitInvocation(InvocationExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitLabel(LabelExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitLambda<T>(Expression<T> node)
		{
			WriteExpressionCommon(node);
			//WriteLine($"[{nameof(LambdaExpression)}] NodeType '{node.NodeType}' (Type '{node.Type.Name}')");
			// todo: visit parameters??
			if (node.Parameters.Count == 0)
			{
				WriteLine("Parameters: none");
			}
			else
			{
				var parameters = node.Parameters
					.Select(p => $"{p.Name} {p.Type.Name}");

				WriteLine($"Parameters: {string.Join(", ", parameters)}");
			}

			WriteLine($"Body: {node.Body.Type.Name}, NodeType '{node.Body.NodeType}'");
			onVisit();
			Visit(node.Body);

			onReturn();
			return node;
		}

		protected override Expression VisitListInit(ListInitExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitLoop(LoopExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitMemberInit(MemberInitExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitNew(NewExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitNewArray(NewArrayExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitSwitch(SwitchExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitTry(TryExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitTypeBinary(TypeBinaryExpression node)
		{

			onReturn();
			return node;
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{

			onReturn();
			return node;
		}

	}
}
