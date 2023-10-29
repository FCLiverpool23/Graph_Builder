using DynamicExpresso;
using Practika;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Practika.Step;
using static Practika.Term;
using static System.Net.Mime.MediaTypeNames;

namespace Practika
{
    public abstract class ParsTreeNode
    {
        public string Value { get; set; }
        public abstract double[] CalculateValue(double[] param);
        public ParsTreeNode Parent { get; set; }
        protected ParsTreeNode(string value, ParsTreeNode parent)
        {
            this.Value = value;
            this.Parent = parent;
        }
    }

    public class Expression : ParsTreeNode
    {
        public enum ExprExp { Plus, Minus, Term }
        static bool IsOperator(char c)
        {
            return c == '-' || c == '+' || c == '*' || c == '/' || c == '^';
        }
        public static bool IsExpression(string expr)
        {
            int oprIndx = -1;
            int brackets = 0;
            for (int i = expr.Length - 1; i > 0; i--)
            {
                if (((expr[i] == '-' && !IsOperator(expr[i - 1]))
                    || expr[i] == '+') && (brackets == 0))
                {
                    oprIndx = i;
                    break;
                }
                else if (expr[i] == ')') brackets++;
                else if (expr[i] == '(') brackets--;
            }
            if (oprIndx > 0)
            {
                string subExpr, term;
                subExpr = expr.Substring(0, oprIndx);
                term = expr.Substring(oprIndx + 1);
                return (Term.IsTerm(term) && IsExpression(subExpr));
            }
            else return Term.IsTerm(expr);
        }
        public ExprExp Exp { get; set; }
        public Term Term { get; set; }
        public Expression SubExpression { get; set; }
        public Expression(string expr, ParsTreeNode parent) : base(expr, parent)
        {
            int oprIndx = -1;
            int brackets = 0;
            for (int i = expr.Length - 1; i > 0; i--)
            {
                if (((expr[i] == '-' && !IsOperator(expr[i - 1]))
                    || expr[i] == '+') && (brackets == 0))
                {
                    oprIndx = i;
                    break;
                }
                else if (expr[i] == ')') brackets++;
                else if (expr[i] == '(') brackets--;
            }
            if (oprIndx > 0)
            {
                string subExpr, term;
                char opr;
                subExpr = expr.Substring(0, oprIndx);
                term = expr.Substring(oprIndx + 1);
                opr = expr[oprIndx];
                this.Term = new Term(term, this);
                this.SubExpression = new Expression(subExpr, this);
                if (opr == '-') Exp = ExprExp.Minus;
                else Exp = ExprExp.Plus;
            }
            else
            {
                Exp = ExprExp.Term;
                this.Term = new Term(expr, this);
            }
        }
        public override double[] CalculateValue(double[] param)
        {
            double res;
            if (Exp == ExprExp.Minus) res = this.SubExpression.CalculateValue(param)[1] - this.Term.CalculateValue(param)[1];
            else if (Exp == ExprExp.Plus) res = this.SubExpression.CalculateValue(param)[1] + this.Term.CalculateValue(param)[1];
            else  res = this.Term.CalculateValue(param)[1];
            param[1] = res;
            return param;
        }
    }

    public class Term : ParsTreeNode
    {
        public enum TermExp { Mul, Div, Step }
        public static bool IsTerm(string term)
        {
            int oprIndx = -1;
            int brackets = 0;
            for (int i = term.Length - 1; i > 0; i--)
            {
                if ((term[i] == '*' || term[i] == '/') && (brackets == 0))
                {
                    oprIndx = i;
                    break;
                }
                else if (term[i] == ')') brackets++;
                else if (term[i] == '(') brackets--;
            }
            if (oprIndx > 0)
            {
                string subterm, factor;
                subterm = term.Substring(0, oprIndx);
                factor = term.Substring(oprIndx + 1);
                return Term.IsTerm(subterm) && Step.IsStep(factor);
            }
            else
            {
                return Step.IsStep(term);
            }
        }
        public TermExp Exp { get; set; }
        public Term SubTerm { get; set; }
        public Step Step { get; set; }
        public Term(string term, ParsTreeNode parent) : base(term, parent)
        {
            this.Value = term;
            int oprIndx = -1;
            int brackets = 0;
            for (int i = term.Length - 1; i > 0; i--)
            {
                if ((term[i] == '*' || term[i] == '/') && (brackets == 0))
                {
                    oprIndx = i;
                    break;
                }
                else if (term[i] == ')') brackets++;
                else if (term[i] == '(') brackets--;
            }
            if (oprIndx > 0)
            {
                string subterm, step;
                char opr = term[oprIndx];
                subterm = term.Substring(0, oprIndx);
                step = term.Substring(oprIndx + 1);
                this.Step = new Step(step, this);
                this.SubTerm = new Term(subterm, this);
                if (opr == '*') this.Exp = TermExp.Mul;
                else this.Exp = TermExp.Div;
            }
            else
            {
                this.Exp = TermExp.Step;
                this.Step = new Step(term, this);
            }
        }
        public override double[] CalculateValue(double[] param)
        {
            double res;
            if (Exp == TermExp.Div)
            {
                if (this.Step.CalculateValue(param)[1] == 0) param[2] = 1;
                res = this.SubTerm.CalculateValue(param)[1] / this.Step.CalculateValue(param)[1];
            }
            else if (Exp == TermExp.Mul) res = this.SubTerm.CalculateValue(param)[1] * this.Step.CalculateValue(param)[1];
            else res = this.Step.CalculateValue(param)[1];
            param[1] = res;
            return param;
        }
    }

    public class Step : ParsTreeNode
    {
        public enum StepExp { Pow, Factor }
        public static bool IsStep(string step)
        {
            int oprIndx = -1;
            int brackets = 0;
            for (int i = step.Length - 1; i > 0; i--)
            {
                if (step[i] == '^' && brackets == 0)
                {
                    oprIndx = i;
                    break;
                }
                else if (step[i] == ')') brackets++;
                else if (step[i] == '(') brackets--;
            }
            if (oprIndx > 0)
            {
                string substep, factor;
                substep = step.Substring(0, oprIndx);
                factor = step.Substring(oprIndx + 1);
                return Step.IsStep(substep) && Factor.IsFactor(factor);
            }
            else return Factor.IsFactor(step);
        }
        public StepExp Exp { get; set; }
        public Step SubStep { get; set; }
        public Factor Factor { get; set; }
        public Step(string step, ParsTreeNode parent) : base(step, parent)
        {
            this.Value = step;
            int oprIndx = -1;
            int brackets = 0;
            for (int i = step.Length - 1; i > 0; i--)
            {
                if (step[i] == '^' && brackets == 0)
                {
                    oprIndx = i;
                    break;
                }
                else if (step[i] == ')') brackets++;
                else if (step[i] == '(') brackets--;
            }
            if (oprIndx > 0)
            {
                string subterm, factor;
                subterm = step.Substring(0, oprIndx);
                factor = step.Substring(oprIndx + 1);
                this.SubStep = new Step(subterm, this);
                this.Factor = new Factor(factor, this);
                this.Exp = StepExp.Pow;
            }
            else
            {
                this.Exp = StepExp.Factor;
                this.Factor = new Factor(step, this);
            }
        }
        public override double[] CalculateValue(double[] param)
        {
            double res;
            if (Exp == StepExp.Pow) res = Math.Pow(this.SubStep.CalculateValue(param)[1], this.Factor.CalculateValue(param)[1]);
            else res = this.Factor.CalculateValue(param)[1];
            param[1] = res;
            return param;
        }
    }

    public class Factor : ParsTreeNode {
        public enum FactorExp { Number, Function, MinuFactor, WrappedExpression, ID , Eps, Pi}
        public static bool IsFactor(string factor)
        {
            double tst;
            if (double.TryParse(factor, out tst))
                return true;
            else if (factor.StartsWith("(") && factor.EndsWith(")") && Expression.IsExpression(factor.Substring(1, factor.Length - 2)))
                return true;
            else if (factor.StartsWith("-") && Factor.IsFactor(factor.Substring(1, factor.Length - 1)))
                return true;
            else if (Function.IsFunction(factor))
                return true;
            else if (factor == "x" || factor == "e" || factor == "𝝅")
                return true;
            else { return false; }
        }
        public FactorExp Expansion { get; set; }
        public Function Function { get; set; }
        public Expression WrappedExpr{ get; set; }
        public Factor InnerFactor;
        public Factor(string factor, ParsTreeNode parent): base(factor, parent)
        {
            this.Value = factor;
            double value;
            if (double.TryParse(factor, out value)) this.Expansion = FactorExp.Number;
            else
            {
                if (factor.StartsWith("(") && factor.EndsWith(")"))
                {
                    this.Expansion = FactorExp.WrappedExpression;
                    this.WrappedExpr = new Expression(factor.Substring(1, factor.Length - 2), this);
                }
                else if (Function.IsFunction(factor))
                {
                    this.Expansion = FactorExp.Function;
                    this.Function = new Function(factor, this);

                }
                else if (factor.StartsWith("-"))
                {
                    this.Expansion = FactorExp.MinuFactor;
                    this.InnerFactor = new Factor(factor.Substring(1, factor.Length - 1), this);
                }
                else if (factor == "e") this.Expansion = FactorExp.Eps;
                else if (factor == "𝝅") this.Expansion = FactorExp.Pi;
                else  this.Expansion = FactorExp.ID;
            }
        }
        public override double[] CalculateValue(double[] param)
        {
            double res;
            if (Expansion == FactorExp.Number) res = double.Parse(this.Value);
            else if (Expansion == FactorExp.WrappedExpression) res = WrappedExpr.CalculateValue(param)[1];
            else if (Expansion == FactorExp.Function) res = this.Function.CalculateValue(param)[1];
            else if (Expansion == FactorExp.MinuFactor) res = -this.InnerFactor.CalculateValue(param)[1];
            else if (Expansion == FactorExp.Eps) res = Math.Exp(1);
            else if (Expansion == FactorExp.Pi) res = Math.PI;
            else res = param[0];
            param[1] = res;
            return param;
        }
    }
    public class Function : ParsTreeNode
    {
        public enum FuncEnum { sinh, cosh, tgh, ctgh, sin, cos, tg, ctg, log, ln, lg, abs, sqrt, asin, acos, atg, actg }
        public static bool IsFunction(string function)
        {
            foreach (string func in Enum.GetNames(typeof(FuncEnum)))
            {
                if (function.StartsWith(func))
                {
                    if (func == "lg")
                    {
                        int oprIndx = 0;
                        for (int i = function.Length - 1; i > 1; i--)
                        {
                            if (function[i] == '(' && (Char.IsDigit(function[i - 1]) || function[i - 1] == ')')) { oprIndx = i; break; }
                        }
                        if (oprIndx == 0) return false;
                        string osn = function.Substring(2, oprIndx - 2);
                        string znach = function.Substring(oprIndx + 1, function.Length - oprIndx - 2);
                        return Expression.IsExpression(osn) && Expression.IsExpression(znach);
                    }
                    else return Expression.IsExpression(function.Substring(func.Length));
                }
            }
            return false;
        }
        public FuncEnum Func { get; set; }
        public Expression Exp, Osn;
        public Function(string function, ParsTreeNode parent): base(function, parent){
            foreach (string func in Enum.GetNames(typeof(FuncEnum)))
            {
                if (function.StartsWith(func))
                {
                    Func = (FuncEnum)Enum.Parse(typeof(FuncEnum), func);
                    if (Func == FuncEnum.lg)
                    {
                        int oprIndx = 0;
                        for (int i = function.Length - 1; i > 0; i--)
                        {
                            if (function[i] == '(' && (Char.IsDigit(function[i - 1]) || function[i - 1] == ')')) { oprIndx = i; break; }
                        }
                        string osn = function.Substring(2, oprIndx-2);
                        string znach = function.Substring(oprIndx + 1, function.Length - oprIndx - 2);
                        this.Exp = new Expression(znach, this);
                        this.Osn = new Expression(osn, this);
                    }
                    else this.Exp = new Expression(function.Substring(func.Length), this);
                    break;
                }
            }
        }
        public override double[] CalculateValue(double[] param)
        {
            double termValue;
            termValue = Exp.CalculateValue(param)[1];
            double ret = 0;
            switch (Func)
            {
                case FuncEnum.sin:
                    ret = Math.Sin(termValue);
                    break;
                case FuncEnum.cos:
                    ret = Math.Cos(termValue);
                    break;
                case FuncEnum.tg:
                    if (((termValue - Math.PI/2) / Math.PI) % 1 == 0) param[2] = 3;
                    ret = Math.Tan(termValue);
                    break;
                case FuncEnum.ctg:
                    if ((termValue / Math.PI) % 1 == 0) param[2] = 4;
                    ret = (1 / Math.Tan(termValue));
                    break;
                case FuncEnum.log:
                    if (termValue <= 0) param[2] = 6;
                    ret = Math.Log10(termValue);
                    break;
                case FuncEnum.ln:
                    if (termValue <= 0) param[2] = 6;
                    ret = Math.Log(termValue, Math.E);
                    break;
                case FuncEnum.lg:
                    double osn = Osn.CalculateValue(param)[1];
                    if (termValue <= 0) param[2] = 6;
                    else if (osn <= 0 || osn == 1) param[2] = 7;
                    ret = Math.Log(termValue, osn);
                    break;
                case FuncEnum.sqrt:
                    if (termValue < 0) param[2] = 2;
                    ret = Math.Sqrt(termValue);
                    break;
                case FuncEnum.abs:
                    ret = Math.Abs(termValue);
                    break;
                case FuncEnum.asin:
                    if ((termValue < -1 || termValue > 1) && param[2] == 0) param[2] = 5; 
                    ret = Math.Asin(termValue);
                    break;
                case FuncEnum.acos:
                    if ((termValue < -1 || termValue > 1) && param[2] == 0) param[2] = 5; 
                    ret = Math.Acos(termValue);
                    break;
                case FuncEnum.atg:
                    ret = Math.Atan(termValue);
                    break;
                case FuncEnum.actg:
                    ret = (Math.PI / 2 - Math.Atan(termValue));
                    break;
                case FuncEnum.sinh:
                    ret = Math.Sinh(termValue);
                    break;
                case FuncEnum.cosh:
                    ret = Math.Cosh(termValue);
                    break;
                case FuncEnum.tgh:
                    ret = Math.Tanh(termValue);
                    break;
                case FuncEnum.ctgh:
                    ret = (1 / Math.Tanh(termValue));
                    break;
            }
            param[1] = ret;
            return param;
        }
    }
}

