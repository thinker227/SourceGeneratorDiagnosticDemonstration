using System;

namespace ConsoleApp {
	class Program {

		static void Main() {
			// Having a non-generated class named "Foo" causes error CS1061.
			// Works as intended.
			new Foo().Write();
		}

	}

	// Generates error 56928 after rebuild and does not cause red squiggles.
	// Renaming class to ex. "Bar" does not cause error to be resolved
	// unless project is rebuilt.
	class Foo {

	}
}
