/* LaTeX Math IDE
Copyright (C) Daniel Jurcau 2013 

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Latex.SymbolExporter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Symbols from http://en.wikibooks.org/wiki/LaTeX/Mathematics
            var processor = new Processor(150);
            var source =
                //@"\alpha \beta \gamma \delta \epsilon \varepsilon \zeta \eta \theta \vartheta \gamma \kappa \lambda \mu \nu \xi \pi \varpi \rho \varrho \sigma \varsigma \tau \upsilon \phi \varphi \chi \psi \omega";
                //@"\Gamma \Delta \Theta \Lambda \Xi \Pi \Sigma \Upsilon \Phi \Psi \Omega"
                //@"\sin \cos \tan \cot \arcsin \arccos \arctan \sinh \cosh \tanh \coth \sec \csc "
                //@"\partial \eth \hbar \imath \jmath \ell \Re \Im \wp \nabla \Box \infty \aleph \beth \gimel",
                //@"| \| / \backslash ( ) [ ] \{ \} \langle \rangle \lceil \rceil \lfloor \rfloor"
                //@"\parallel \nparallel \models \mid"
                //@"\rightarrow \leftarrow \mapsto \implies \Rightarrow \leftrightarrow \iff \Leftrightarrow \uparrow \Uparrow \downarrow \Downarrow"
                //@"\exists \nexists \forall \neg \subset \supset \subseteq \supseteq \sqsubset \sqsupset \sqsubseteq \sqsupseteq \cap \cup \uplus \sqcap \sqcup \in \notin \ni \land \lor \emptyset \varnothing \top \bot"
                //@"\pm \mp \times \div \ast \star \dagger \ddagger \cdot \diamond \bigtriangleup \bigtriangledown \triangleleft \triangleright \bigcirc \bullet \wr \oplus \ominus \otimes \oslash \odot \circ \setminus \amalg"
                @"< > \leq \geq \ll \gg \prec \succ \preceq \succeq = \doteq \equiv \approx \cong \simeq \sim \propto \neq \parallel \nparallel \asymp \bowtie \vdash \dashv \smile \frown \models \mid"
            ;
            var symbols = source.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder();

            foreach (var item in symbols)
            {
                result.AppendLine(processor.Process(item));
                Console.WriteLine(item);
            }

            Clipboard.SetText(result.ToString());

            processor.Dispose();
        }
    }
}
