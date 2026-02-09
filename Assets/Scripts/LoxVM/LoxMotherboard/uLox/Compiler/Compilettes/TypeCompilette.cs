using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ULox
{
    public enum TypeCompiletteStage 
    { 
        Invalid, 
        Begin, 
        Static, 
        Mixin,
        Signs,
        Var, 
        Init, 
        Method, 
        Complete 
    }
    
    public enum UserType : byte
    {
        Native,
        Class,
        Enum,
    }

    public abstract class TypeCompilette : ICompilette
    {
        public abstract TokenType MatchingToken { get; }

        protected TypeCompiletteStage Stage = TypeCompiletteStage.Invalid;
        protected TypeInfoEntry _currentTypeInfo;
        public TypeInfoEntry CurrentTypeInfoEntry => _currentTypeInfo;

        public string CurrentTypeName => _currentTypeInfo?.Name ?? null;
        public event System.Action<Compiler> OnPostBody;
        public Label InitChainLabelId { get; protected set; } = Label.Default;
        public Label PreviousInitFragLabelId { get; set; } = Label.Default;
        public bool IsReadOnlyAtEnd { get; set; } = false;

        public void CName(Compiler compiler, bool canAssign)
        {
            var cname = CurrentTypeName;
            compiler.AddConstantStringAndWriteOp(cname);
        }

        public abstract UserType UserType { get; }
        public abstract bool EmitClosureCallAtEnd { get; }
        protected abstract void Start();
        protected abstract void DoDeclareType(Compiler compiler);
        protected abstract void InnerBodyElement(Compiler compiler);

        public void Process(Compiler compiler)
        {
            PreviousInitFragLabelId = Label.Default;

            Start();            

            DoDeclareType(compiler);

            DoClassBody(compiler);

            DoInitChainEnd(compiler);
            DoEndType(compiler);
            
            OnPostBody?.Invoke(compiler);
            OnPostBody = null;

            _currentTypeInfo = null;
            InitChainLabelId = Label.Default;
            PreviousInitFragLabelId = Label.Default;
        }

        

        
        private void DoDeclareTypeOLD(Compiler compiler)
        {
            Stage = TypeCompiletteStage.Begin;
            compiler.TokenIterator.Consume(TokenType.IDENTIFIER, "Expect Class name.");
            _currentTypeInfo = new TypeInfoEntry((string)compiler.TokenIterator.PreviousToken.Literal, UserType);
            compiler.TypeInfo.AddType(_currentTypeInfo, compiler);

            compiler.PushCompilerState($"{CurrentTypeName}", FunctionType.TypeDeclare);
            byte nameConstant = compiler.AddStringConstant();

            //InitChainLabelId = compiler.CreateUniqueChunkLabel($"{Chunk.InternalLabelPrefix}InitChain");
            //compiler.EmitPacket(new ByteCodePacket(OpCode.FETCH_GLOBAL, nameConstant));
            compiler.EmitPacket(new ByteCodePacket(OpCode.CLASS, nameConstant));
            compiler.DefineVariable(nameConstant);

            /*
             Token className = Tokens.Consume(IDENTIFIER, "Expect class name.");
            int nameConstant = MakeVarNameConstant(className.Lexeme); // no fixup needed
            DeclareVariable(className); // The class name binds the class object type to a variable of the same name.
            // todo? make the class declaration an expression, require explicit binding of class to variable (like var Pie = new Pie()); 27.2
            EmitOpcode(className.Line, OP_CLASS);
            EmitConstantIndex(className.Line, nameConstant, null); // got rid of fixup 19-02-2025
            DefineVariable(className.Line, MakeVarNameConstant(className.Lexeme)); // no fixup needed
            _CurrentClass = new LoxCompilerClass(className, _CurrentClass);
            // superclass:
            if (Tokens.Match(LESS)) {
                Tokens.Consume(IDENTIFIER, "Expect superclass name.");
                if (Tokens.Previous().Lexeme == className.Lexeme) {
                    throw new CompilerException(Tokens.Previous(), "A class cannot inherit from itself.");
                }
                NamedVariable(Tokens.Previous(), false); // push super class onto stack
                BeginScope();
                AddLocal(MakeSyntheticToken(SUPER, "super", LineOfLastToken));
                DefineVariable(LineOfLastToken, 0);
                NamedVariable(className); // push class onto stack
                EmitOpcode(className.Line, OP_INHERIT);
                _CurrentClass.HasSuperClass = true;
            }
            NamedVariable(className); // push class onto stack
             */
            /// ToDo:
            /// 
            /// the inheritance
            if (compiler.TokenIterator.Match(TokenType.COLON))
            {
                compiler.TokenIterator.Consume(TokenType.IDENTIFIER, "Expect Superclass Name");
                var supername = compiler.TokenIterator.PreviousToken.Literal;
                if (supername == _currentTypeInfo.Name) 
                    compiler.ThrowCompilerException ("A class cannot inherit from itself.");
                var targetTypeInfoEntry = compiler.TypeInfo.GetUserType(supername, compiler);
                if (targetTypeInfoEntry.UserType != UserType.Class) 
                    compiler.ThrowCompilerException("A Class con only Inherit from a Class.");
                _currentTypeInfo.AddSuper (targetTypeInfoEntry);
                byte superConstant = compiler.AddStringConstant();
                compiler.NamedVariable(supername,false);
                compiler.NamedVariable(_currentTypeInfo.Name,false);
                compiler.EmitPacket(new ByteCodePacket(OpCode.INHERIT));
            }

            compiler.TokenIterator.Consume(TokenType.OPEN_BRACE, "Expect '{' before type body.");
        }

        private void DoEndType(Compiler compiler)
        {
            compiler.TokenIterator.Consume(TokenType.CLOSE_BRACE, "Expect '}' after class body.");
            var chunk = compiler.EndCompile();
            if (EmitClosureCallAtEnd)
            {
                compiler.EmitPacket(new ByteCodePacket(new ByteCodePacket.ClosureDetails(
                        ClosureType.Closure, 
                        compiler.CurrentChunk.AddConstant(Value.New(chunk)), 
                        (byte)chunk.UpvalueCount))
                );
                compiler.EmitPacket(new ByteCodePacket(OpCode.CALL, 0, 0, 0));
                compiler.EmitPop();
            }
        }

        private void DoInitChainEnd(Compiler compiler)
        {
            //return stub used by init and test chains
            var classReturnEnd = compiler.GotoUniqueChunkLabel("ClassReturnEnd");

            if (PreviousInitFragLabelId != Label.Default)
            {
                compiler.EmitLabel(PreviousInitFragLabelId);
                _currentTypeInfo.PrependInitChain(compiler.CurrentChunk, InitChainLabelId);
            }

            compiler.EmitPacket(new ByteCodePacket(OpCode.RETURN));

            compiler.EmitLabel(classReturnEnd);
        }

        private void DoClassBody(Compiler compiler)
        {
            while (!compiler.TokenIterator.Check(TokenType.CLOSE_BRACE) && !compiler.TokenIterator.Check(TokenType.EOF))
            {
                InnerBodyElement(compiler);
            }

            Stage = TypeCompiletteStage.Complete;
        }
    }
}
