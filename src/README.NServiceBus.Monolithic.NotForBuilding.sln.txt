##BEWARE!!!
##Do not attempt to build this solution. NServiceBus has a separate build script. The results of building this solution are unclear.
##It is not certain that this solution remains entirely up to date. For the definitive truth open the respective small solutions
##
##If you refactor using this solution be aware that you will refactor straight across public private boundaries.
##Your build and tests may work while all users out there making use of your incompatible interfaces will break when updating.


The NServiceBus structure of many solutions is good for separation of concerns, dependency management and for automated builds. 
However, it is not good for the ability to browse and understand the code. Trying to make sense of the code in large can be quite a daunting task with dozens of solutions.
This solution is meant to help in that regard.
It may also be very helpful for refactoring. BUT take care about the warning above!

This solution has been created by recreating a solution folder for each folder in NServiceBus\src. To avoid conflicts in later steps and during updates the names are prefixed with an underscore _
Each solution in each NServiceBus\src sub folder was added to this solution and the result including solution folders was moved into the corresponding solution folder.
for NServiceBus\src\ subfolders like impl and such solution folder was created and the process applied recursively
This is intended to make each folder in this solution mirror the structure of the solution in the corresponding physical directory rather than physical directory structures.


#Exceptions:
	NServiceBus\src\impl\Persistence\RavenPersistence  Reason: No solution present in any subfolder. 
	Instead NServiceBus.Persistence.Raven.csprj and NServiceBus.Persistence.Raven.Tests.csprj were included in several other solutions.
	I removed them from those solution paths and placed them here for consistency

	NServiceBus\src\utils\NServiceBus.Utils present also in other solution. Kept in utils where it seems to belong.


