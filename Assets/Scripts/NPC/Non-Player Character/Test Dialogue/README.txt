--How to use the dialogue system

First you need to create a new .txt document using the required format below. 
The name of this text document will be how this piece of dialogue is referenced by other dialogues and is referred to as the DialogueID.

After the "END" keyword, the different choices can be added optionally. If not choices are placed after the END, then the dialogue box will close automatically
after all the chunks have been presented.

Format:

DialogueID,PortraitID,PortraitID,SFXID,DialogueBoxName,Text
....
{You can have any number of these separated by newlines.}
....
END

NextDialogueID,TallyScore,ChoiceName
{You can have any number of these separated by newlines.}

Structure of the dialogue class:
Each dialogue is composed of a list of dialogue chunks, which are a data class defining each sentences with the portraits and sound effects.

Implimenting in code:

Adding a new dialogue to the system:
    For a dialogue to be accessible, it *****must***** be added to the dialogue array inside the DialogueSystem gameobject.
    As of right now, the dialogue parser from text to object trusts the input entirely, so a malformed input will crash the game immediately.
    Only edit the outputs of the helper program manually if you're fully confident in the format above, or the edit is simple.

Adding dialogue prompts in game:
    Dialogue is prompted to start via the StartDialogue method in the DialogueSystem singleton. Starting a dialogue while one is going will automatically fail.