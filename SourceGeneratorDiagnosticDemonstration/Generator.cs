using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable RS2008

namespace SourceGeneratorDiagnosticDemonstration {
	/// <summary>
	/// Source generator demonstrating Roslyn issue #56928.
	/// https://github.com/dotnet/roslyn/issues/56928
	/// </summary>
	[Generator]
	public class Generator : ISourceGenerator {

		private static readonly DiagnosticDescriptor descriptor = new(
			"56928",
			"Types should not be named Foo",
			"Type {0} is named Foo",
			"Naming",
			DiagnosticSeverity.Error,
			true
		);



		// Generates a class named Foo and reports an error
		// if a class with said name already exists.
		public void Execute(GeneratorExecutionContext context) {
			// Get declared named types
			List<INamedTypeSymbol> types = new();
			var trees = context.Compilation.SyntaxTrees;
			foreach (var tree in trees) {
				var semanticModel = context.Compilation.GetSemanticModel(tree);
				var typeSymbols = tree.GetRoot().DescendantNodes()
					.OfType<TypeDeclarationSyntax>()
					.Select(t => (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(t));
				types.AddRange(typeSymbols);
			}

			// Report error 56928 on types named "Foo"
			bool errored = false;
			foreach (var type in types) {
				if (type.Name == "Foo") {
					var diagnostic = Diagnostic.Create(
						descriptor,
						type.Locations.First(),
						$"{type.ContainingNamespace}.{type.Name}"
					);
					context.ReportDiagnostic(diagnostic);
					errored = true;
				}
			}
			if (errored) return;

			string source =
@"// Auto generated
namespace ConsoleApp {
	public class Foo {
		
		public void Write() =>
			System.Console.WriteLine(""Foo"");
		
	}
}
";

			context.AddSource("FooGeneratedSource", source);
		}

		public void Initialize(GeneratorInitializationContext context) { }
	
	}
}
