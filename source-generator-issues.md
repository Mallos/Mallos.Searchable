# Source Generator issues
## Summary
[Source Generators Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md)

When working with the source generators I hit a lot of issues.
Here is a collection of the issues I hit which forced me to make
other design decisions which I wouldn't otherwise.

## Issues
* Can only read the currently referenced project, would be nice to get a better overview of type usage.
* Unable to get more information from external attributes, than the name and argument list.
  * Would be nice to be able to visit the syntax nodes in external libraries too.
  * Only option I see here is to use reflection, but that creates other issues.
